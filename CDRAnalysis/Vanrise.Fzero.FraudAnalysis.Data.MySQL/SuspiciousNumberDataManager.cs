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

        public void SaveSuspiciousNumbers(List<SuspiciousNumber> suspiciousNumbers)
        {
            string filename = GetFilePathForBulkInsert();
            string sFields = "";


                for (int i = 1; i <= 18; i++)
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

                    for (int i = 1; i <= 18; i++)
                    {
                        if (suspiciousNumber.CriteriaValues.Where(x => x.Key == i).Count() == 1)
                        {
                            sValues = sValues + ", '" + suspiciousNumber.CriteriaValues.Where(x => x.Key == i).FirstOrDefault().Value.ToString() +"'";
                        }
                    }

                    streamWriter.WriteLine("'0', '" + suspiciousNumber.DateDay.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + suspiciousNumber.Number + "' " + sValues + ", '" + suspiciousNumber.SuspectionLevel.ToString() + "', '" + suspiciousNumber.StrategyId.ToString() + "', NULL");

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


        public void SaveNumberProfiles(List<NumberProfile> numberProfiles)
        {
            string filename = GetFilePathForBulkInsert();

            using (StreamWriter streamWriter = new StreamWriter(filename))
            {
                foreach (NumberProfile numberProfile in numberProfiles)
                {

                    streamWriter.WriteLine("0,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32}",
                                 new[] 
                             
                                 { 
                                    numberProfile.SubscriberNumber.ToString(),	
                                    numberProfile.FromDate.ToString(),	
                                    numberProfile.ToDate.ToString()	,
                                    numberProfile.AggregateValues["CountOutCalls"].ToString()	,
                                    numberProfile.AggregateValues["DiffOutputNumb"].ToString()	,	
                                    numberProfile.AggregateValues["CountOutInters"].ToString()	,	
                                    numberProfile.AggregateValues["CountInInters"].ToString()	,
                                    "0",	
                                    "0",		
                                    "0",		
                                    numberProfile.AggregateValues["CallOutDurs"].ToString(),	
                                    "0",	
                                    "0",		
                                    numberProfile.AggregateValues["CountOutFails"].ToString()	,
                                    numberProfile.AggregateValues["CountInFails"].ToString()	,
                                    numberProfile.AggregateValues["TotalOutVolume"].ToString()	,
                                    numberProfile.AggregateValues["TotalInVolume"].ToString(),	
                                    numberProfile.AggregateValues["DiffInputNumbers"].ToString()	,
                                    numberProfile.AggregateValues["CountOutSMSs"].ToString()	,
                                    numberProfile.AggregateValues["TotalIMEI"].ToString()	,	
                                    numberProfile.AggregateValues["TotalBTS"].ToString()	,	
                                    numberProfile.IsOnNet.ToString()	,	
                                    numberProfile.AggregateValues["TotalDataVolume"].ToString()	,
                                    ((int)numberProfile.Period).ToString()		,
                                    numberProfile.AggregateValues["CountInCalls"].ToString()	,
                                    numberProfile.AggregateValues["CallInDurs"].ToString()	,	
                                    numberProfile.AggregateValues["CountOutOnNets"].ToString()	,	
                                    numberProfile.AggregateValues["CountInOnNets"].ToString()	,
                                    numberProfile.AggregateValues["CountOutOffNets"].ToString()	,
                                    numberProfile.AggregateValues["CountInOffNets"].ToString()	,
	                                numberProfile.AggregateValues["CountFailConsecutiveCalls"].ToString(),
	                                numberProfile.AggregateValues["CountConsecutiveCalls"].ToString(),
	                                numberProfile.AggregateValues["CountOutLowDurationCalls"].ToString()			
                                 }
                   );




                }
                streamWriter.Close();
            }

            MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString());
            string query = String.Format(@"LOAD DATA LOCAL  INFILE '{0}' INTO TABLE NumberProfile FIELDS TERMINATED BY ',' LINES TERMINATED BY '\r'   ;", filename.Replace(@"\", @"\\"));
            mySqlConnection.Open();
            MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection);
            mySqlCommand.CommandTimeout = int.MaxValue;
            mySqlCommand.ExecuteNonQuery();
            mySqlConnection.Close();
        }


    }
}
