using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class SQLManager : BaseSQLDataManager
    {
        public SQLManager()
            : base("CDRDBConnectionString")
        {

        }

        public void ExecuteReader(String query,Action<SqlCommand> prepareCmd , Action<SqlDataReader> onReaderReady)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionString()))
            {
                connection.Open();

                using (SqlCommand cmd = new SqlCommand(query, connection))
                {
                    cmd.CommandTimeout = int.MaxValue;
                    prepareCmd(cmd);

                    using (SqlDataReader dataReader = cmd.ExecuteReader())
                    {
                        onReaderReady(dataReader);
                        dataReader.Close();
                    }
                }
                connection.Close();
            }
        }

        public List<T> GetItems<T>(string query, Action<SqlCommand> prepareCmd , Func<SqlDataReader, T> builder)
        {
            List<T> rslt = new List<T>();
            ExecuteReader(query, prepareCmd, (reader) =>
                {
                    while (reader.Read())
                        rslt.Add(builder(reader));
                });
            return rslt;
        }

        public T GetItem<T>(string query, Action<SqlCommand> prepareCmd, Func<SqlDataReader, T> builder)
        {
            List<T> rslt = new List<T>();
            ExecuteReader(query, prepareCmd, (reader) =>
            {
                while (reader.Read())
                    rslt.Add(builder(reader));
            });
            return rslt.FirstOrDefault();
        }
    }
}
