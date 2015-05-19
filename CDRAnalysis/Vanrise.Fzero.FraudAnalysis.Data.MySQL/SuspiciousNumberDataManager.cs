using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Vanrise.Data.MySQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

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
            string sFields = "";


                for (int i = 1; i <= 15; i++)
                {
                    if (suspiciousNumbers.FirstOrDefault().CriteriaValues.Where(x => x.Key == i).Count() == 1)
                    {
                        sFields = sFields + ", Criteria" + i.ToString();
                    }
                }



                using (StreamWriter streamWriter = new StreamWriter(filename))
            {
                foreach (SuspiciousNumber suspiciousNumber in suspiciousNumbers)
                {
                    string sValues = "";

                    for (int i = 1; i <= 15; i++)
                    {
                        if (suspiciousNumber.CriteriaValues.Where(x => x.Key == i).Count() == 1)
                        {
                            sValues = sValues + ", '" + suspiciousNumber.CriteriaValues.Where(x => x.Key == i).FirstOrDefault().Value.ToString() +"'";
                        }
                    }

                    streamWriter.WriteLine("'0', '" + suspiciousNumber.DateDay.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + suspiciousNumber.Number + "' " + sValues + ", '" + suspiciousNumber.SuspectionLevel.ToString() + "', '" + strategy.Id.ToString() + "', '" + ((int)suspiciousNumber.Period).ToString() + "'");

                }
                streamWriter.Close();
            }

                MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString());
            string query = String.Format(@"LOAD DATA LOCAL  INFILE '{0}' INTO TABLE SubscriberThresholds FIELDS TERMINATED BY ',' LINES TERMINATED BY '\r'  (Id, DateDay, SubscriberNumber " + sFields + ", SuspectionLevelId, StrategyId, PeriodId)  ;", filename.Replace(@"\", @"\\"));
            mySqlConnection.Open();
            MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection);
            mySqlCommand.CommandTimeout = int.MaxValue;
            mySqlCommand.ExecuteNonQuery();
            mySqlConnection.Close();
        }

    }
}
