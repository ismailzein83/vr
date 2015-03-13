﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;
using System.IO;

namespace Vanrise.Data.SQL
{
    public class BaseSQLDataManager : BaseDataManager
    {
        static BaseSQLDataManager()
        {
            AddBCPIfNotAdded();
        }

        static string s_bcpDirectory;
        private static void AddBCPIfNotAdded()
        {
            s_bcpDirectory = Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetAssembly(typeof(BaseSQLDataManager)).Location), "BCPRoot");
            if (!Directory.Exists(s_bcpDirectory))
                Directory.CreateDirectory(s_bcpDirectory);
            string bcpFullPath = Path.Combine(s_bcpDirectory, "v_bcp.exe");
            if(!File.Exists(bcpFullPath))
                File.WriteAllBytes(bcpFullPath, Resource.v_bcp);
            
            string bcprllFullPath = Path.Combine(s_bcpDirectory, "bcp.rll");
            if(!File.Exists(bcprllFullPath))
                File.WriteAllBytes(bcprllFullPath, Resource.bcp);
        }

        #region ctor

        public BaseSQLDataManager()
            : base()
        {
        }

        public BaseSQLDataManager(string connectionStringKey) : base(connectionStringKey)
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

        #region DataTable

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

        protected string GetFilePathForBulkInsert()
        {
            return System.IO.Path.GetTempFileName();// String.Format(@"C:\CodeMatch\{0}.txt", Guid.NewGuid());
        }

        protected void InsertBulkToTable(BulkInsertInfo bulkInsertInfo)
        {
            if (bulkInsertInfo == null)
                throw new ArgumentNullException("bulkInsertInfo");
            if (String.IsNullOrEmpty(bulkInsertInfo.DataFilePath))
                throw new ArgumentNullException("bulkInsertInfo.DataFilePath");
            if (String.IsNullOrEmpty(bulkInsertInfo.TableName))
                throw new ArgumentNullException("bulkInsertInfo.TableName");
            if(bulkInsertInfo.FieldSeparator == default(char))
                throw new ArgumentNullException("bulkInsertInfo.FieldSeparator");



            string errorFilePath = System.IO.Path.GetTempFileName();// String.Format(@"C:\CodeMatch\Error\{0}.txt", Guid.NewGuid());
            SqlConnectionStringBuilder connStringBuilder = new SqlConnectionStringBuilder(GetConnectionString());
            StringBuilder args = new StringBuilder(String.Format("{0} in {1} -e {2} -c -d {3} -S {4} -t {5} -b 100000 -F2", bulkInsertInfo.TableName, bulkInsertInfo.DataFilePath, errorFilePath, connStringBuilder.InitialCatalog, connStringBuilder.DataSource, bulkInsertInfo.FieldSeparator));
            
            if (connStringBuilder.IntegratedSecurity)
                args.Append(" -T");
            else
                args.Append(String.Format(" -U {0} -P {1}", connStringBuilder.UserID, connStringBuilder.Password));
            
            if (bulkInsertInfo.TabLock)
                args.Append(" -h TABLOCK");
            if (bulkInsertInfo.KeepIdentity)
                args.Append(" -E");
            
            System.Diagnostics.Process processBulkCopy = new System.Diagnostics.Process();

            var procStartInfo = new System.Diagnostics.ProcessStartInfo(Path.Combine(s_bcpDirectory, "v_bcp"), args.ToString());
            
            procStartInfo.RedirectStandardOutput = true;
            procStartInfo.UseShellExecute = false;
            procStartInfo.CreateNoWindow = true;
            processBulkCopy.StartInfo = procStartInfo;
            processBulkCopy.Start();
            processBulkCopy.WaitForExit();
            string errorMessage = File.ReadAllText(errorFilePath);
            File.Delete(bulkInsertInfo.DataFilePath);
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
    }
}
