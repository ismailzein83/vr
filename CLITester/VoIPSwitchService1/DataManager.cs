using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;

namespace VoIPSwitchService
{
    public class DataManager
    {
        public void GetData(int prefixLength, GeneratedCall generatedCall, Action<MySqlDataReader> onReaderReady)
        {

            MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection();
            conn.ConnectionString = ConfigurationManager.ConnectionStrings["MySQL"].ToString();
            conn.Open();

            int numberLength = generatedCall.Number.IndexOf("@") - prefixLength;
            string number = generatedCall.Number.Substring(prefixLength, numberLength);

            string query = String.Format(@"SELECT * FROM voipswitch.callsfailed c where id_client = 2 and ip_number='192.168.22.28' and called_number = '555{0}'
                    and (call_start BETWEEN (SELECT DATE_ADD('{1}', INTERVAL (SELECT TIMESTAMPDIFF(SECOND,'{2}'
                ,NOW())) SECOND)) AND (SELECT DATE_ADD('{3}', INTERVAL (SELECT TIMESTAMPDIFF(SECOND,'{2}'
                ,NOW())) SECOND))) order by id_failed_call desc limit 20;", number, generatedCall.StartDate.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                                                                          DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), generatedCall.EndDate.Value.ToString("yyyy-MM-dd HH:mm:ss"));

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
