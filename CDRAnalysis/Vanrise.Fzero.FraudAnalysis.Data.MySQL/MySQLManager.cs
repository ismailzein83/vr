using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.MySQL;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class MySQLManager : BaseMySQLDataManager
    {
        public MySQLManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }

        public void ExecuteReader(String query,Action<MySqlCommand> prepareCmd , Action<MySqlDataReader> onReaderReady)
        {
            using (MySqlConnection connection = new MySqlConnection(GetConnectionString()))
            {
                connection.Open();

                using (MySqlCommand cmd = new MySqlCommand(query, connection))
                {
                    prepareCmd(cmd);

                    using (MySqlDataReader dataReader = cmd.ExecuteReader())
                    {
                        onReaderReady(dataReader);
                        dataReader.Close();
                    }
                }
                connection.Close();
            }
        }

        public List<T> GetItems<T>(string query, Action<MySqlCommand> prepareCmd , Func<MySqlDataReader, T> builder)
        {
            List<T> rslt = new List<T>();
            ExecuteReader(query, prepareCmd, (reader) =>
                {
                    while (reader.Read())
                        rslt.Add(builder(reader));
                });
            return rslt;
        }

        public T GetItem<T>(string query, Action<MySqlCommand> prepareCmd, Func<MySqlDataReader, T> builder)
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
