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

                    for (int i = 1; i <= 15; i++)
                    {
                        if (cdr.CriteriaValues.Where(x => x.Key == i).Count()==1)
                        {
                            s = s + "," + cdr.CriteriaValues.Where(x => x.Key == i).FirstOrDefault().Value.ToString();
                        }
                        else
                        {
                            s = s + ", null";
                        }
                    }

                    sw.WriteLine("0, null, "+cdr.Number+" " + s + ", " + cdr.SuspectionLevel.ToString() + ", " + strategy.Id.ToString() + ", 6");

                    //Id, DateDay, SubscriberNumber, Criteria1, Criteria2, Criteria3, Criteria4, Criteria5, Criteria6, Criteria7, Criteria8, Criteria9, Criteria10, Criteria11, Criteria12, Criteria13, Criteria14, Criteria15, SuspectionLevelId, StrategyId, PeriodId

                    
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
