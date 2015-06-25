using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using MySql.Data.MySqlClient;
using System.Configuration;

namespace PhoneNumbersService
{
    public class MySQLDataManager
    {
        public void GetData(string number, DateTime creationDate, DateTime releaseDate, Action<MySqlDataReader> onReaderReady)
        {

            MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["MySQL"].ToString();
            conn.Open();

            string query = String.Format(@"SELECT * FROM voipswitch.callsfailed c where id_client = 2 and ip_number='192.168.22.28' and called_number = '555{0}'
                    and (call_start BETWEEN (SELECT DATE_ADD('{1}', INTERVAL (SELECT TIMESTAMPDIFF(SECOND,'{2}'
                ,NOW())) SECOND)) AND (SELECT DATE_ADD('{3}', INTERVAL (SELECT TIMESTAMPDIFF(SECOND,'{2}'
                ,NOW())) SECOND))) order by id_failed_call desc limit 20;", number, creationDate.ToString("yyyy-MM-dd HH:mm:ss"),
                                                                          DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), releaseDate.ToString("yyyy-MM-dd HH:mm:ss"));

            MySqlCommand cmd = new MySqlCommand(query, conn);

            using (MySqlDataReader reader = cmd.ExecuteReader())
            {
                onReaderReady(reader);
                reader.Close();
            }
            conn.Close();
        }
    }
}