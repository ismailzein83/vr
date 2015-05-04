using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Data;
using MySql.Data.MySqlClient;
using System.IO;
using Vanrise.Data.MySQL;

namespace Vanrise.Fzero.FraudAnalysis.Data.MySQL
{
    public class SuspiciousNumberDataManager : BaseMySQLDataManager, ISuspiciousNumberDataManager
    {
        public SuspiciousNumberDataManager()
            : base("CDRDBConnectionStringMySQL")
        {

        }

        public void SaveSuspiciousNumbers(List<SuspiciousNumber> suspiciousNumbers, Strategy strategy)
        {
            string filename = GetFilePathForBulkInsert();

            using (StreamWriter sw = new StreamWriter(filename))
            {
                foreach (SuspiciousNumber cdr in suspiciousNumbers)
                {
                    string s = "";
                    foreach (KeyValuePair<int, Decimal> pair in cdr.CriteriaValues)
                    {
                        s = "," + s + pair.Value;
                    }

                    sw.WriteLine("0,{0},{1}{2},{3},{4},{5}",
                                  new[]  {null, cdr.Number, s, cdr.SuspectionLevel.ToString(), 
                                       strategy.Id.ToString(), ""}
                    );
                }
                sw.Close();
            }

            MySqlConnection connection = new MySqlConnection(GetConnectionString());
            string query = String.Format(@"LOAD DATA LOCAL  INFILE '{0}' INTO TABLE SubscriberThresholds FIELDS TERMINATED BY ',' LINES TERMINATED BY '\r';", filename.Replace(@"\", @"\\"));
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.CommandTimeout = int.MaxValue;
            cmd.ExecuteNonQuery();
            connection.Close();
        }

    }
}
