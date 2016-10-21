using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

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
        protected void ExecuteReaderText(string cmdText, Action<IDataReader> onReaderReady, Action<DbCommand> prepareCommand)
        {
            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(this.GetConnectionString()))
            {
                if (conn.State == ConnectionState.Closed) conn.Open();
                using (var cmd = CreateCommand(conn, cmdText))
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

        #region ExecuteScalar

        protected object ExecuteScalarText(string cmdText, Action<DbCommand> prepareCommand)
        {
            object result;
            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(this.GetConnectionString()))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                using (var cmd = CreateCommand(conn, cmdText))
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

        protected int ExecuteNonQueryText(string cmdText, Action<DbCommand> prepareCommand)
        {
            int rowsAffected;
            using (Npgsql.NpgsqlConnection conn = new Npgsql.NpgsqlConnection(this.GetConnectionString()))
            {
                if (conn.State == ConnectionState.Closed)
                    conn.Open();
                using (var cmd = CreateCommand(conn, cmdText))
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
        private DbCommand CreateCommand(DbConnection conn, string sql)
        {
            var cmd = conn.CreateCommand();
            cmd.CommandText = sql;
            cmd.CommandTimeout = 600;
            return cmd;
        }

        #endregion
    }
}
