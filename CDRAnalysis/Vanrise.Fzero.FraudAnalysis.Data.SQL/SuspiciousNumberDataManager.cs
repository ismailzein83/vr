using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Fzero.FraudAnalysis.Entities;
using Vanrise.Data;
using MySql.Data.MySqlClient;
using System.IO;
using Vanrise.Data.SQL;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class SuspiciousNumberDataManager : BaseSQLDataManager, ISuspiciousNumberDataManager
    {
        public SuspiciousNumberDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public void SaveSuspiciousNumbers(List<SuspiciousNumber> suspiciousNumbers, Strategy strategy)
        {
            string filename = GetFilePathForBulkInsert();
            string sFields = "";


                for (int i = 1; i <= 15; i++)
                {
                    if (suspiciousNumbers.FirstOrDefault().CriteriaValues.Where(x => x.Key == i).Count() == 1)
                    {
                        sFields = sFields + ", Criteria" + i.ToString();
                    }
                }



            using (StreamWriter sw = new StreamWriter(filename))
            {
                foreach (SuspiciousNumber sn in suspiciousNumbers)
                {
                    string sValues = "";

                    for (int i = 1; i <= 15; i++)
                    {
                        if (sn.CriteriaValues.Where(x => x.Key == i).Count() == 1)
                        {
                            sValues = sValues + ", '" + sn.CriteriaValues.Where(x => x.Key == i).FirstOrDefault().Value.ToString() +"'";
                        }
                    }

                    sw.WriteLine("'0', '" + sn.DateDay.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + sn.Number + "' " + sValues + ", '" + sn.SuspectionLevel.ToString() + "', '" + strategy.Id.ToString() + "', '" + sn.PeriodId.ToString() +"'");


                    //Id, DateDay, SubscriberNumber, Criteria1, Criteria2, Criteria3, Criteria4, Criteria5, Criteria6, Criteria7, Criteria8, Criteria9, Criteria10, Criteria11, Criteria12, Criteria13, Criteria14, Criteria15, SuspectionLevelId, StrategyId, PeriodId
                }
                sw.Close();
            }

            MySqlConnection connection = new MySqlConnection(GetConnectionString());
            string query = String.Format(@"LOAD DATA LOCAL  INFILE '{0}' INTO TABLE SubscriberThresholds FIELDS TERMINATED BY ',' LINES TERMINATED BY '\r'  (Id, DateDay, SubscriberNumber " + sFields + ", SuspectionLevelId, StrategyId, PeriodId)  ;", filename.Replace(@"\", @"\\"));
            connection.Open();
            MySqlCommand cmd = new MySqlCommand(query, connection);
            cmd.CommandTimeout = int.MaxValue;
            cmd.ExecuteNonQuery();
           connection.Close();
        }

    }
}
