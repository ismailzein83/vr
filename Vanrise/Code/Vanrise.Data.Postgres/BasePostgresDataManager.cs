using Npgsql;
using System;
using System.Collections.Generic;
using System.Data;

namespace Vanrise.Data.Postgres
{
    public class BasePostgresDataManager : BaseDataManager
    {
        #region ctor

        public BasePostgresDataManager()
            : base()
        {

        }

        public BasePostgresDataManager(string connectionStringKey)
            : base(connectionStringKey)
        {

        }

        #endregion

        #region Get Reader Field

        protected T GetReaderValue<T>(IDataReader reader, string fieldName)
        {
            return reader[fieldName] != DBNull.Value ? (T)reader[fieldName] : default(T);
        }

        #endregion

        #region ExecuteReader

        protected void ExecuteReaderText(string cmdText, Action<IDataReader> onReaderReady, Action<NpgsqlCommand> prepareCommand, int? commandTimeout = null)
        {
            using (NpgsqlConnection conn = new NpgsqlConnection(this.GetConnectionString()))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                using (var cmd = CreateCommand(conn, cmdText, commandTimeout))
                {
                    if (prepareCommand != null)
                        prepareCommand(cmd);

                    using (var reader = cmd.ExecuteReader())
                    {
                        onReaderReady(reader);
                        reader.Close();
                    }
                }
                conn.Close();
            }
        }

        #endregion

        #region GetItem(s)

        protected T GetItemText<T>(string cmdText, Func<IDataReader, T> objectBuilder, Action<NpgsqlCommand> prepareCommand)
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

        protected List<T> GetItemsText<T>(string cmdText, Func<IDataReader, T> objectBuilder, Action<NpgsqlCommand> prepareCommand)
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

        #region ExecuteScalar

        protected object ExecuteScalarText(string cmdText, Action<NpgsqlCommand> prepareCommand, int? commandTimeout = null)
        {
            object result;
            using (NpgsqlConnection conn = new NpgsqlConnection(this.GetConnectionString()))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                using (var cmd = CreateCommand(conn, cmdText, commandTimeout))
                {
                    if (prepareCommand != null)
                        prepareCommand(cmd);

                    result = cmd.ExecuteScalar();
                }
                conn.Close();
            }
            return result;
        }

        #endregion

        #region ExecuteNonQuery

        public void ExecuteNonQuery(string[] sqlStrings, int? commandTimeout = null)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();
                foreach (string sql in sqlStrings)
                {
                    using (NpgsqlCommand command = CreateCommand(connection, sql, commandTimeout))
                    {
                        command.ExecuteNonQuery();
                    }
                }
                connection.Close();
            }
        }

        public void ExecuteNonQuery(Func<string> getNextQuery, int? commandTimeout = null)
        {
            using (NpgsqlConnection connection = new NpgsqlConnection(GetConnectionString()))
            {
                if (connection.State == ConnectionState.Closed) connection.Open();

                while (true)
                {
                    string query = getNextQuery();
                    if (string.IsNullOrEmpty(query))
                        break;

                    using (NpgsqlCommand command = CreateCommand(connection, query, commandTimeout))
                    {
                        command.ExecuteNonQuery();
                    }
                }

                connection.Close();
            }
        }

        protected int ExecuteNonQueryText(string cmdText, Action<NpgsqlCommand> prepareCommand, int? commandTimeout = null)
        {
            int rowsAffected;
            using (NpgsqlConnection conn = new NpgsqlConnection(this.GetConnectionString()))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                using (var cmd = CreateCommand(conn, cmdText, commandTimeout))
                {
                    if (prepareCommand != null)
                        prepareCommand(cmd);

                    rowsAffected = cmd.ExecuteNonQuery();
                }
                conn.Close();
            }
            return rowsAffected;
        }

        #endregion

        #region Private Functions

        private NpgsqlCommand CreateCommand(NpgsqlConnection conn, string sql, int? commandTimeout)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandTimeout = commandTimeout.HasValue ? commandTimeout.Value : 600;
            return cmd;
        }

        #endregion

        #region BulkCopy

        public void Bulk(IEnumerable<INpgBulkCopy> objectList, string tableName)
        {
            if (objectList == null)
                return;

            using (NpgsqlConnection conn = new NpgsqlConnection(this.GetConnectionString()))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                using (var inStream = conn.BeginTextImport(string.Format("COPY {0} FROM STDIN", tableName)))
                {
                    foreach (var objectItem in objectList)
                        inStream.WriteLine(objectItem.ConvertToString());
                }
            }
        }

        #endregion
    }
}