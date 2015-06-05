using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Diagnostics;

namespace VoIPSwitchTestApp
{
    public class MySQLConn
    {
        public void start()
        {
            try
            {
                string myConnectionString;
                DateTime dFrom = new DateTime(2014, 01, 19, 14, 54, 59);
                DateTime dTo = new DateTime(2016, 01, 19, 14, 54, 59);
                string dateFrom = dFrom.ToString("yyyy-MM-dd HH:mm:ss");
                string dateTo = dTo.ToString("yyyy-MM-dd HH:mm:ss");

                myConnectionString = "server=" + ConfigurationManager.AppSettings["serverIP"] +
                    ";uid=" + ConfigurationManager.AppSettings["uid"] + ";" + "Persist Security Info=False;password=" +
                    ConfigurationManager.AppSettings["pwd"] + ";database=" + ConfigurationManager.AppSettings["database"] + ";";

                MySql.Data.MySqlClient.MySqlConnection conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = myConnectionString;
                conn.Open();
                WriteToEventLog("myConnectionString: " + myConnectionString);
                //int prefixLength = testoperators[i].CarrierPrefix.Length;//6
                ////int numberLength = GenEnd.Number.Length;//15
                ////int numberLength = GenEnd.Number.IndexOf("@");
                ////WriteToEventLog("prefixLength: " + prefixLength + " numberLength: " + numberLength + " number: " + GenEnd.Number);
                ////string number = GenEnd.Number.Substring(prefixLength, numberLength);
                string number = "96171910273";

                string query = "SELECT * FROM voipswitch.callsfailed c where id_client = 2 and ip_number='192.168.22.28' and called_number = '555" + number +
                    "' and (call_start BETWEEN '" + dateFrom + "' AND '" + dateTo + "') order by id_failed_call desc limit 20;";

                MySqlCommand cmd = new MySqlCommand(query, conn);

                MySqlDataReader reader = cmd.ExecuteReader();
                if (reader.HasRows)
                {
                    Console.WriteLine(reader[0]);
                }
                conn.Close();
            }
            catch (System.Exception ex)
            {
                WriteToEventLog(ex.ToString());
            }
        }

        private void WriteToEventLog(string message)
        {
            string cs = "Android Service";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }
    }
}
