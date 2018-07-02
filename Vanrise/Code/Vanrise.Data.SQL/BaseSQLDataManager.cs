using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Configuration;
using Vanrise.Entities;
using System.Collections.Concurrent;
using System.Text.RegularExpressions;

namespace Vanrise.Data.SQL
{
    public class BaseSQLDataManager : BaseDataManager
    {
        static string _bcpCommandName;
        static string s_bcpDirectory;

        #region ctor

        static BaseSQLDataManager()
        {
            _bcpCommandName = ConfigurationManager.AppSettings["BCPCommandName"];
            AddBCPIfNotAdded();
        }

        public BaseSQLDataManager()
            : base()
        {
        }

        public BaseSQLDataManager(string connectionStringKey)
            : base(connectionStringKey)
        {
        }

        public BaseSQLDataManager(string connectionString, bool getFromConfigSection)
            : base(connectionString, getFromConfigSection)
        {

        }

        #endregion

        #region Public Methods

        protected DateTimeRange GetSQLDateTimeRange()
        {
            return new DateTimeRange()
            {
                From = new DateTime(1753, 1, 1, 0, 0, 0),
                To = new DateTime(9999, 12, 31, 23, 59, 59)
            };
        }

        protected int GetSQLQueryMaxParameterNumber()
        {
            return 2000; //Exactly 2100
        }

        public string GetSqlParameterSqlType(string parameterName, Object parameterValue)
        {
            SqlParameter sqlPrm = new SqlParameter(parameterName, parameterValue);
            string sqlType = sqlPrm.SqlDbType.ToString();
            if (sqlPrm.Size > 0)
                sqlType = string.Format("{0}({1})", sqlType, sqlPrm.Size);
            else if (sqlPrm.SqlDbType == SqlDbType.Decimal)
                sqlType = "Decimal(38, 15)";
            return sqlType;
        }

        #endregion

        #region ExecuteNonQuery

        protected int ExecuteNonQuerySP(string spName, params object[] parameters)
        {
            SqlDatabase db = CreateDatabase();
            int rowsAffected = 0;
            using (var cmd = CreateCommandFromSP(db, spName))
            {
                db.AssignParameters(cmd, parameters.ToArray());
                rowsAffected = db.ExecuteNonQuery(cmd);
            }
            return rowsAffected;
        }

        protected int ExecuteNonQuerySP(string spName, out object outputPrm, params object[] parameters)
        {
            int rowsAffected = 0;
            SqlDatabase db = CreateDatabase();
            using (var cmd = CreateCommandFromSP(db, spName))
            {
                List<object> paramtersWithOutput = new List<object>();
                paramtersWithOutput.AddRange(parameters);
                paramtersWithOutput.Add(DBNull.Value);
                db.AssignParameters(cmd, paramtersWithOutput.ToArray());
                rowsAffected = db.ExecuteNonQuery(cmd);
                outputPrm = cmd.Parameters[cmd.Parameters.Count - 1].Value;
            }
            return rowsAffected;
        }

        protected int ExecuteNonQuerySPCmd(string spName, Action<DbCommand> prepareCommand)
        {
            SqlDatabase db = CreateDatabase();
            int rowsAffected = 0;
            using (var cmd = CreateCommandFromSP(db, spName))
            {
                if (prepareCommand != null)
                    prepareCommand(cmd);
                rowsAffected = db.ExecuteNonQuery(cmd);
            }
            return rowsAffected;
        }

        protected int ExecuteNonQueryText(string cmdText, Action<DbCommand> prepareCommand, int? commandTimeoutInSeconds = null)
        {
            int rowsAffected;
            var db = CreateDatabase();
            using (var cmd = CreateCommand(db, cmdText, commandTimeoutInSeconds))
            {
                if (prepareCommand != null)
                    prepareCommand(cmd);

                rowsAffected = db.ExecuteNonQuery(cmd);
            }
            return rowsAffected;
        }

        #endregion

        #region ExecuteScalar

        protected object ExecuteScalarSP(string spName, params object[] parameters)
        {
            SqlDatabase db = CreateDatabase();
            object val = null;
            using (var cmd = CreateCommandFromSP(db, spName))
            {
                db.AssignParameters(cmd, parameters.ToArray());
                val = db.ExecuteScalar(cmd);
            }
            return val;
        }

        protected object ExecuteScalarText(string cmdText, Action<DbCommand> prepareCommand)
        {
            object val = null;
            var db = CreateDatabase();
            using (var cmd = CreateCommand(db, cmdText))
            {
                if (prepareCommand != null)
                    prepareCommand(cmd);

                val = db.ExecuteScalar(cmd);
            }
            return val;
        }

        #endregion

        #region ExecuteReader

        protected void ExecuteReaderText(string cmdText, Action<IDataReader> onReaderReady, Action<DbCommand> prepareCommand)
        {
            var db = CreateDatabase();
            using (var cmd = CreateCommand(db, cmdText))
            {
                if (prepareCommand != null)
                    prepareCommand(cmd);

                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    onReaderReady(reader);
                    cmd.Cancel();
                    reader.Close();
                }
            }
        }

        protected void ExecuteReaderSP(string spName, Action<IDataReader> onReaderReady, params object[] parameters)
        {
            var db = CreateDatabase();
            using (var cmd = CreateCommandFromSP(db, spName))
            {
                db.AssignParameters(cmd, parameters.ToArray());
                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    onReaderReady(reader);
                    cmd.Cancel();
                    reader.Close();
                }
            }

        }

        protected void ExecuteReaderSPCmd(string spName, Action<IDataReader> onReaderReady, Action<DbCommand> prepareCommand)
        {
            var db = CreateDatabase();
            using (var cmd = CreateCommandFromSP(db, spName))
            {
                if (prepareCommand != null)
                    prepareCommand(cmd);

                using (IDataReader reader = db.ExecuteReader(cmd))
                {
                    onReaderReady(reader);
                    cmd.Cancel();
                    reader.Close();
                }
            }
        }

        #endregion

        #region GetItem(s)

        protected T GetItemSP<T>(string spName, Func<IDataReader, T> objectBuilder, params object[] parameters)
        {
            T obj = default(T);
            ExecuteReaderSP(spName, (reader) =>
            {
                if (reader.Read())
                {
                    obj = objectBuilder(reader);
                }
            },
                parameters);
            return obj;
        }

        protected T GetItemSPCmd<T>(string spName, Func<IDataReader, T> objectBuilder, Action<DbCommand> prepareCommand)
        {
            T obj = default(T);
            ExecuteReaderSPCmd(spName,
                (reader) =>
                {
                    if (reader.Read())
                    {
                        obj = objectBuilder(reader);
                    }
                },
                prepareCommand);
            return obj;
        }

        protected T GetItemText<T>(string cmdText, Func<IDataReader, T> objectBuilder, Action<DbCommand> prepareCommand)
        {
            T obj = default(T);
            ExecuteReaderText(cmdText, (reader) =>
            {
                if (reader.Read())
                {
                    obj = objectBuilder(reader);
                }
            },
                prepareCommand);
            return obj;
        }

        protected List<T> GetItemsSP<T>(string spName, Func<IDataReader, T> objectBuilder, params object[] parameters)
        {
            List<T> lst = new List<T>();
            ExecuteReaderSP(spName, (reader) =>
            {
                while (reader.Read())
                {
                    T obj = objectBuilder(reader);
                    if (obj != null)
                        lst.Add(obj);
                }
            },
                parameters);
            return lst;
        }

        protected List<T> GetItemsSPCmd<T>(string spName, Func<IDataReader, T> objectBuilder, Action<DbCommand> prepareCommand)
        {
            List<T> lst = new List<T>();
            ExecuteReaderSPCmd(spName,
                (reader) =>
                {
                    while (reader.Read())
                    {
                        T obj = objectBuilder(reader);
                        if (obj != null)
                            lst.Add(obj);
                    }
                },
                prepareCommand);
            return lst;
        }

        protected List<T> GetItemsText<T>(string cmdText, Func<IDataReader, T> objectBuilder, Action<DbCommand> prepareCommand)
        {
            List<T> lst = new List<T>();
            ExecuteReaderText(cmdText, (reader) =>
            {
                while (reader.Read())
                {
                    T obj = objectBuilder(reader);
                    if (obj != null)
                        lst.Add(obj);
                }
            },
                prepareCommand);
            return lst;
        }

        #endregion

        #region Bulk Insert

        protected void WriteDataTableToDB(DataTable table, SqlBulkCopyOptions options, bool withBatchSize = true)
        {
            if (table.Rows.Count == 0)
                return;
            using (var conn = GetOpenConnection())
            {
                SqlBulkCopy bulkCopy = new SqlBulkCopy(conn, options, null);
                bulkCopy.DestinationTableName = table.TableName;
                foreach (DataColumn column in table.Columns)
                    bulkCopy.ColumnMappings.Add(column.ColumnName, column.ColumnName);
                bulkCopy.BulkCopyTimeout = 10 * 60; // 10 minutes
                if (withBatchSize)
                    bulkCopy.BatchSize = 1000;

                bulkCopy.WriteToServer(table);
                conn.Close();
            }
            table.Dispose();
        }

        protected StreamForBulkInsert InitializeStreamForBulkInsert()
        {
            string filePath = GetFilePathForBulkInsert();
            return new StreamForBulkInsert(filePath);
        }

        protected string GetFilePathForBulkInsert()
        {
            string configuredDirectory = ConfigurationManager.AppSettings["BCPTempFilesDirectory"];
            if (!String.IsNullOrEmpty(configuredDirectory))
            {
                string filePath = Path.Combine(configuredDirectory, Guid.NewGuid().ToString());
                using (var stream = File.Create(filePath))
                {
                    stream.Close();
                }
                return filePath;
            }
            else
                return System.IO.Path.GetTempFileName();// String.Format(@"C:\CodeMatch\{0}.txt", Guid.NewGuid());
        }

        protected bool GetDeleteBCPFiles()
        {
            string donotDeleteFiles = ConfigurationManager.AppSettings["BCPDonotDeleteFiles"];
            if (!String.IsNullOrEmpty(donotDeleteFiles))
            {

                return !bool.Parse(donotDeleteFiles);
            }
            else
                return true;
        }

        protected void InsertBulkToTable(BaseBulkInsertInfo bulkInsertInfo)
        {
            if (bulkInsertInfo == null)
                throw new ArgumentNullException("bulkInsertInfo");

            string dataFilePath = bulkInsertInfo.GetDataFilePath();
            if (String.IsNullOrEmpty(dataFilePath))
                throw new ArgumentNullException("bulkInsertInfo.GetDataFilePath()");
            if (String.IsNullOrEmpty(bulkInsertInfo.TableName))
                throw new ArgumentNullException("bulkInsertInfo.TableName");
            if (bulkInsertInfo.FieldSeparator == default(char))
                throw new ArgumentNullException("bulkInsertInfo.FieldSeparator");

            string sqlPassword;

            //string baseBCPArgs = GetBaseBCPArgs(bulkInsertInfo, "in " + dataFilePath, out sqlPassword);
            string baseBCPArgs = GetBaseBCPArgs(bulkInsertInfo, string.Format("in \"{0}\"", dataFilePath), out sqlPassword);

            string bcpFormatArgs = GetBCPFormatArgs(bulkInsertInfo);

            StringBuilder args = new StringBuilder(String.Format("{0} {1} -t {2} -b 100000", baseBCPArgs, bcpFormatArgs, bulkInsertInfo.FieldSeparator));

            if (bulkInsertInfo.TabLock)
                args.Append(" -h TABLOCK");
            if (bulkInsertInfo.KeepIdentity)
                args.Append(" -E");

            ExecuteBCPCommand(args.ToString(), sqlPassword);
            if (GetDeleteBCPFiles())
                File.Delete(dataFilePath);
        }

        static ConcurrentDictionary<string, string> s_bcpFormatFileNamesByTable = new ConcurrentDictionary<string, string>();

        private string GetBCPFormatArgs(BaseBulkInsertInfo bulkInsertInfo)
        {
            if (bulkInsertInfo.ColumnNames == null)
                return "";

            SqlConnectionStringBuilder connStringBuilder = new SqlConnectionStringBuilder(GetConnectionString());
            string tableUniqueIndentifierName = string.Concat(connStringBuilder.InitialCatalog, ".", bulkInsertInfo.TableName);

            string formatFileName;
            if (s_bcpFormatFileNamesByTable.TryGetValue(tableUniqueIndentifierName, out formatFileName) && File.Exists(formatFileName))
                return string.Format("-f {0}", formatFileName);

            formatFileName = System.IO.Path.GetTempFileName();

            string sqlPassword;
            string baseBCPArgs = GetBaseBCPArgs(bulkInsertInfo, " format nul ", out sqlPassword);

            ExecuteBCPCommand(String.Format("{0} -f {1}", baseBCPArgs, formatFileName), sqlPassword);
            ApplyColumnsToBCPFormatFile(formatFileName, bulkInsertInfo);
            s_bcpFormatFileNamesByTable.AddOrUpdate(tableUniqueIndentifierName, formatFileName, (k, v) => formatFileName);
            return string.Format("-f {0}", formatFileName);
        }

        private void ApplyColumnsToBCPFormatFile(string formatFileName, BaseBulkInsertInfo bulkInsertInfo)
        {
            List<string> formatFileLines = File.ReadLines(formatFileName).ToList();
            Dictionary<string, BCPFormatFileLine> columns = new Dictionary<string, BCPFormatFileLine>();
            for (int i = 2; i < formatFileLines.Count; i++)
            {
                string line = formatFileLines[i];
                line = ReplaceEmptyStrings(line, "|");
                string[] lineFields = line.Split('|');
                BCPFormatFileLine col = new BCPFormatFileLine
                {
                    HostFileDataType = lineFields[1],
                    ServerColumnOrder = lineFields[5],
                    ServerColumnName = lineFields[6],
                    ColumnCollation = lineFields[7]
                };
                columns.Add(col.ServerColumnName, col);
            }

            List<string> newFormatLines = new List<string>();
            newFormatLines.Add(formatFileLines[0]);
            newFormatLines.Add(bulkInsertInfo.ColumnNames.Count().ToString());
            int columnOrder = 0;
            foreach (string columnName in bulkInsertInfo.ColumnNames)
            {
                BCPFormatFileLine matchColumn;
                if (!columns.TryGetValue(columnName, out matchColumn))
                    throw new Exception(String.Format("Column Name '{0}' is not available in the SQL table '{1}'", columnName, bulkInsertInfo.TableName));
                columnOrder++;
                string fieldDelimiter = String.Format("\"{0}\"", (columnOrder < bulkInsertInfo.ColumnNames.Count() ? bulkInsertInfo.FieldSeparator.ToString() : "\\r\\n"));
                newFormatLines.Add(String.Format("{0}  {1}  0  0  {2}  {3}  {4}  {5}", columnOrder, matchColumn.HostFileDataType, fieldDelimiter, matchColumn.ServerColumnOrder, matchColumn.ServerColumnName, matchColumn.ColumnCollation));
            }
            File.WriteAllLines(formatFileName, newFormatLines);
        }

        private string GetBaseBCPArgs(BaseBulkInsertInfo bulkInsertInfo, string bcpCommand, out string sqlPassword)
        {
            SqlConnectionStringBuilder connStringBuilder = new SqlConnectionStringBuilder(GetConnectionString());
            string securityArgs;
            sqlPassword = null;
            if (connStringBuilder.IntegratedSecurity)
                securityArgs = " -T";
            else
            {
                securityArgs = String.Format(" -U {0} -P #PASSWORD# ", connStringBuilder.UserID);
                sqlPassword = connStringBuilder.Password;
            }
            return String.Format("{0} {1} -d {2} -S {3} {4} -c ", bulkInsertInfo.TableName, bcpCommand, connStringBuilder.InitialCatalog, connStringBuilder.DataSource, securityArgs);
        }

        private void ExecuteBCPCommand(string args, string sqlPassword)
        {
            string bcpPath;
            if (string.IsNullOrEmpty(_bcpCommandName))
                bcpPath = Path.Combine(s_bcpDirectory, "v_bcp");
            else
                bcpPath = _bcpCommandName;

            System.Diagnostics.Process processBulkCopy = new System.Diagnostics.Process();

            string argsWithPassword = sqlPassword != null ? args.Replace("#PASSWORD#", sqlPassword) : args;
            var procStartInfo = new System.Diagnostics.ProcessStartInfo(bcpPath, argsWithPassword);
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            processBulkCopy.StartInfo = procStartInfo;

            bool isStarted = processBulkCopy.Start();
            processBulkCopy.WaitForExit();
            string output = processBulkCopy.StandardOutput.ReadToEnd();
            if (!isStarted || processBulkCopy.ExitCode != 0 || (output != null && output.Contains("Error")))
            {
                Exception ex = new Exception(String.Format("Error When running BCP Command. \r\n\r\nBCP Path: {0}. \r\n\r\nBCP Arguments: {1}. \r\n\r\nBCP Output: {2}", bcpPath, args, output));
                Vanrise.Common.LoggerFactory.GetExceptionLogger().WriteException(ex);
                throw ex;
            }

        }

        private string ReplaceEmptyStrings(string line, string characterToReplace)
        {
            RegexOptions options = RegexOptions.None;
            Regex regex = new Regex(@"[ ]{2,}", options);
            return regex.Replace(line, characterToReplace);
        }


        /// <summary>
        /// check https://msdn.microsoft.com/en-us/library/ms179250.aspx for BCP Format file structure
        /// </summary>
        private class BCPFormatFileLine
        {
            public string HostFileDataType { get; set; }

            public string ServerColumnOrder { get; set; }

            public string ServerColumnName { get; set; }

            public string ColumnCollation { get; set; }
        }

        #endregion

        #region Private Methods

        private static void AddBCPIfNotAdded()
        {
            s_bcpDirectory = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(BaseSQLDataManager)).Location), "BCPRoot");
            if (!Directory.Exists(s_bcpDirectory))
                Directory.CreateDirectory(s_bcpDirectory);
            string bcpFullPath = Path.Combine(s_bcpDirectory, "v_bcp.exe");
            if (!File.Exists(bcpFullPath))
                File.WriteAllBytes(bcpFullPath, Resource.v_bcp);

            string bcprllFullPath = Path.Combine(s_bcpDirectory, "bcp.rll");
            if (!File.Exists(bcprllFullPath))
                File.WriteAllBytes(bcprllFullPath, Resource.bcp);
        }

        private SqlConnection GetOpenConnection()
        {
            SqlConnection connection = new System.Data.SqlClient.SqlConnection(GetConnectionString());
            connection.Open();
            //return connection;            
            //IDbConnection connection = DataConfiguration.Default.SessionFactory.ConnectionProvider.GetConnection();
            using (IDbCommand command = connection.CreateCommand())
            {
                command.CommandText =
                    @"set ANSI_NULLS ON 
                  set ANSI_PADDING ON 
                  set ANSI_WARNINGS ON 
                  set ARITHABORT ON 
                  set CONCAT_NULL_YIELDS_NULL ON 
                  set QUOTED_IDENTIFIER ON 
                  set NUMERIC_ROUNDABORT OFF";
                command.ExecuteNonQuery();
            }

            return connection;
        }

        private SqlDatabase CreateDatabase()
        {
            SqlDatabase db = new SqlDatabase(GetConnectionString());
            return db;
        }

        private DbCommand CreateCommand(SqlDatabase db, string sql, int? commandTimeoutInSeconds = null)
        {
            var cmd = db.GetSqlStringCommand(sql);
            cmd.CommandTimeout = commandTimeoutInSeconds.HasValue ? commandTimeoutInSeconds.Value : 600;
            return cmd;
        }

        private DbCommand CreateCommandFromSP(SqlDatabase db, string spName)
        {
            var cmd = db.GetStoredProcCommand(spName);
            cmd.CommandTimeout = 600;
            return cmd;
        }

        #endregion

        #region Temp Table


        protected TempTableName GenerateTempTableName()
        {
            string tableName = Guid.NewGuid().ToString().Replace("-", "");
            return new TempTableName
            {
                Key = tableName,
                TableName = String.Format("tempdb.dbo.t_{0}", tableName)
            };
        }

        protected TempTableName GetTempTableName(string tableNameKey)
        {
            return new TempTableName
            {
                Key = tableNameKey,
                TableName = String.Format("tempdb.dbo.t_{0}", tableNameKey)
            };
        }

        protected IEnumerable<T> GetDataFromTempTable<T>(string tempTableName, int fromRow, int toRow, string orderByColumnName, bool isDescending, Func<IDataReader, T> objectBuilder, out int totalCount)
        {
            string query = String.Format(@"WITH OrderedResult AS (SELECT *, ROW_NUMBER()  OVER ( ORDER BY {1} {2}) AS rowNumber FROM {0})
	                                    SELECT * FROM OrderedResult WHERE rowNumber between @FromRow AND @ToRow", tempTableName, orderByColumnName, isDescending ? "DESC" : "ASC");

            totalCount = (int)ExecuteScalarText(String.Format("SELECT COUNT(*) FROM {0}", tempTableName), null);
            return GetItemsText(query,
                objectBuilder,
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@FromRow", fromRow));
                    cmd.Parameters.Add(new SqlParameter("@ToRow", toRow));
                });
        }

        protected IEnumerable<T> GetAllDataFromTempTable<T>(string tempTableName, string orderByColumnName, bool isDescending, Func<IDataReader, T> objectBuilder)
        {
            string query = String.Format(@"SELECT * FROM {0} ORDER BY {1} {2}", tempTableName, orderByColumnName, isDescending ? "DESC" : "ASC");

            return GetItemsText(query,
                objectBuilder,
                (cmd) =>
                {
                });
        }

        protected BigResult<T> RetrieveData<T>(DataRetrievalInput input, Action<string> createTempTableIfNotExists, Func<IDataReader, T> objectBuilder, Dictionary<string, string> fieldsToColumnsMapper = null, BigResult<T> rslt = null)
        {
            TempTableName tempTableName = null;
            if (!String.IsNullOrWhiteSpace(input.ResultKey))
                tempTableName = GetTempTableName(input.ResultKey);
            else
                tempTableName = GenerateTempTableName();

            if (rslt == null)
                rslt = new BigResult<T>();
            rslt.ResultKey = tempTableName.Key;

            createTempTableIfNotExists(tempTableName.TableName);
            string sortByColumnName = input.SortByColumnName;
            if (fieldsToColumnsMapper != null && fieldsToColumnsMapper.ContainsKey(sortByColumnName))
                sortByColumnName = fieldsToColumnsMapper[sortByColumnName];
            if (input.FromRow.HasValue && input.ToRow.HasValue)
            {
                int totalDataCount;
                rslt.Data = GetDataFromTempTable(tempTableName.TableName, input.FromRow.Value, input.ToRow.Value, sortByColumnName, input.IsSortDescending, objectBuilder, out totalDataCount);
                rslt.TotalCount = totalDataCount;
            }
            else
            {
                rslt.Data = GetAllDataFromTempTable(tempTableName.TableName, sortByColumnName, input.IsSortDescending, objectBuilder);
                rslt.TotalCount = rslt.Data.Count();
            }
            return rslt;
        }

        #endregion

        #region Caching Methods

        protected bool IsDataUpdated(string tableName, ref object lastReceivedDataInfo)
        {
            string query = String.Format("SELECT MAX(timestamp) FROM {0} WITH(NOLOCK)", tableName);
            var newReceivedDataInfo = ExecuteScalarText(query, null);
            return IsDataUpdated(ref lastReceivedDataInfo, newReceivedDataInfo);
        }

        protected bool IsDataUpdated<T>(string tableName, string columnName, T columnValue, ref object lastReceivedDataInfo)
        {
            string query = String.Format("SELECT MAX(timestamp) FROM {0} WITH(NOLOCK) WHERE {1} = @ColumnValue", tableName, columnName);
            var newReceivedDataInfo = ExecuteScalarText(query, (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@ColumnValue", columnValue));
                });
            return IsDataUpdated(ref lastReceivedDataInfo, newReceivedDataInfo);
        }

        protected bool IsDataUpdated(ref object lastReceivedDataInfo, object newReceivedDataInfo)
        {
            if (newReceivedDataInfo == null)
                return false;
            else
            {
                byte[] newTimeStamp = (byte[])newReceivedDataInfo;
                if (lastReceivedDataInfo == null || !newTimeStamp.SequenceEqual((byte[])lastReceivedDataInfo))
                {
                    lastReceivedDataInfo = newTimeStamp;
                    return true;
                }
                else
                    return false;
            }
        }

        #endregion
    }
}