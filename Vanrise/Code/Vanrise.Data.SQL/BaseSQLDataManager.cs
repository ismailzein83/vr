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

namespace Vanrise.Data.SQL
{
    public class BaseSQLDataManager : BaseDataManager
    {
        static string _bcpCommandName;
        static BaseSQLDataManager()
        {
            _bcpCommandName = ConfigurationManager.AppSettings["BCPCommandName"];
            AddBCPIfNotAdded();
        }

        static string s_bcpDirectory;
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

        #region ctor

        public BaseSQLDataManager()
            : base()
        {
        }

        public BaseSQLDataManager(string connectionStringKey)
            : base(connectionStringKey)
        {
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

        protected int ExecuteNonQueryText(string cmdText, Action<DbCommand> prepareCommand)
        {
            var db = CreateDatabase();
            using (var cmd = CreateCommand(db, cmdText))
            {
                if (prepareCommand != null)
                    prepareCommand(cmd);

                return db.ExecuteNonQuery(cmd);
            }
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
            var db = CreateDatabase();
            using (var cmd = CreateCommand(db, cmdText))
            {
                if (prepareCommand != null)
                    prepareCommand(cmd);

                return db.ExecuteScalar(cmd);
            }
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
                    bulkCopy.BatchSize = 10000;

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



            string errorFilePath = System.IO.Path.GetTempFileName();// String.Format(@"C:\CodeMatch\Error\{0}.txt", Guid.NewGuid());
            SqlConnectionStringBuilder connStringBuilder = new SqlConnectionStringBuilder(GetConnectionString());
            StringBuilder args = new StringBuilder(String.Format("{0} in {1} -e {2} -c -d {3} -S {4} -t {5} -b 100000", bulkInsertInfo.TableName, dataFilePath, errorFilePath, connStringBuilder.InitialCatalog, connStringBuilder.DataSource, bulkInsertInfo.FieldSeparator));

            if (connStringBuilder.IntegratedSecurity)
                args.Append(" -T");
            else
                args.Append(String.Format(" -U {0} -P {1}", connStringBuilder.UserID, connStringBuilder.Password));

            if (bulkInsertInfo.TabLock)
                args.Append(" -h TABLOCK");
            if (bulkInsertInfo.KeepIdentity)
                args.Append(" -E");

            System.Diagnostics.Process processBulkCopy = new System.Diagnostics.Process();

            string bcpPath;
            if (string.IsNullOrEmpty(_bcpCommandName))
                bcpPath = Path.Combine(s_bcpDirectory, "v_bcp");
            else
                bcpPath = _bcpCommandName;

            var procStartInfo = new System.Diagnostics.ProcessStartInfo(bcpPath, args.ToString());
           
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            processBulkCopy.StartInfo = procStartInfo;
            processBulkCopy.Start();
            processBulkCopy.WaitForExit();
            string errorMessage = File.ReadAllText(errorFilePath);
            if (GetDeleteBCPFiles())
                File.Delete(dataFilePath);
            File.Delete(errorFilePath);
            if (!String.IsNullOrWhiteSpace(errorMessage))
                throw new Exception(errorMessage);
        }

        #endregion

        #region Private Methods

        SqlConnection GetOpenConnection()
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

        private DbCommand CreateCommand(SqlDatabase db, string sql)
        {
            var cmd = db.GetSqlStringCommand(sql);
            cmd.CommandTimeout = 600;
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

        protected BigResult<T> RetrieveData<T>(DataRetrievalInput input, Action<string> createTempTableIfNotExists, Func<IDataReader, T> objectBuilder, Dictionary<string,string> fieldsToColumnsMapper = null, BigResult<T> rslt = null)
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

        bool IsDataUpdated(ref object lastReceivedDataInfo, object newReceivedDataInfo)
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
