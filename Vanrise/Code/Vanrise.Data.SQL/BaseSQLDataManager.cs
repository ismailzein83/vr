using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Data;
using Microsoft.Practices.EnterpriseLibrary.Data.Sql;
using System.Data.Common;
using System.Data;
using System.Data.SqlClient;

namespace Vanrise.Data.SQL
{
    public class BaseSQLDataManager : BaseDataManager
    {
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

        protected void ExecuteReaderSPCmd(string spName, Action<DbCommand> prepareCommand, Action<IDataReader> onReaderReady)
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

        #endregion

        #region Private Methods

        SqlConnection GetOpenConnection()
        {
            SqlConnection connection = new System.Data.SqlClient.SqlConnection(base._connectionString);
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
            SqlDatabase db = new SqlDatabase(_connectionString);
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
