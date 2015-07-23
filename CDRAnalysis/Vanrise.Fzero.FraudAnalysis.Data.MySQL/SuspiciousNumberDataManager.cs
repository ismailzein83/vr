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

                    streamWriter.WriteLine("'0', '" + suspiciousNumber.DateDay.Value.ToString("yyyy-MM-dd HH:mm:ss") + "', '" + suspiciousNumber.Number + "' " + sValues + ", '" + suspiciousNumber.SuspectionLevel.ToString() + "', '" + suspiciousNumber.StrategyId.ToString() + "' ");

                }
                streamWriter.Close();
            }

                MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString());
                string query = String.Format(@"LOAD DATA LOCAL  INFILE '{0}' INTO TABLE FraudAnalysis.SubscriberThreshold FIELDS TERMINATED BY ',' LINES TERMINATED BY '\r'  (Id, DateDay, SubscriberNumber " + sFields + ", SuspicionLevelId, StrategyId)  ;", filename.Replace(@"\", @"\\"));
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

                    streamWriter.WriteLine("0,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33}",
                                 new[] 
                             
                                 { 
                                      numberProfile.SubscriberNumber.ToString(),	
                                    numberProfile.FromDate.ToString(),	
                                    numberProfile.ToDate.ToString()	,
                                    Math.Round(numberProfile.AggregateValues["CountOutCalls"],2).ToString()	,
                                    Math.Round(numberProfile.AggregateValues["DiffOutputNumb"],2).ToString()	,	
                                    Math.Round(numberProfile.AggregateValues["CountOutInters"],2).ToString()	,	
                                    Math.Round(numberProfile.AggregateValues["CountInInters"],2).ToString()	,
                                    "0",	
                                    "0",		
                                    "0",		
                                    Math.Round(numberProfile.AggregateValues["CallOutDurAvg"],2).ToString(),	
                                    "0",	
                                    "0",		
                                    Math.Round(numberProfile.AggregateValues["CountOutFails"],2).ToString()	,
                                    Math.Round(numberProfile.AggregateValues["CountInFails"],2).ToString()	,
                                    Math.Round(numberProfile.AggregateValues["TotalOutVolume"],2).ToString()	,
                                    Math.Round(numberProfile.AggregateValues["TotalInVolume"],2).ToString(),	
                                    Math.Round(numberProfile.AggregateValues["DiffInputNumbers"],2).ToString()	,
                                    Math.Round(numberProfile.AggregateValues["CountOutSMSs"],2).ToString()	,
                                    Math.Round(numberProfile.AggregateValues["TotalIMEI"],2).ToString()	,	
                                    Math.Round(numberProfile.AggregateValues["TotalBTS"],2).ToString()	,	
                                    numberProfile.IsOnNet.ToString()	,	
                                    Math.Round(numberProfile.AggregateValues["TotalDataVolume"],2).ToString()	,
                                    ((int)numberProfile.PeriodId).ToString()		,
                                    Math.Round(numberProfile.AggregateValues["CountInCalls"],2).ToString()	,
                                    Math.Round(numberProfile.AggregateValues["CallInDurAvg"],2).ToString()	,	
                                    Math.Round(numberProfile.AggregateValues["CountOutOnNets"],2).ToString()	,	
                                    Math.Round(numberProfile.AggregateValues["CountInOnNets"],2).ToString()	,
                                    Math.Round(numberProfile.AggregateValues["CountOutOffNets"],2).ToString()	,
                                    Math.Round(numberProfile.AggregateValues["CountInOffNets"],2).ToString() ,
	                                Math.Round(numberProfile.AggregateValues["CountFailConsecutiveCalls"],2).ToString(),
	                                Math.Round(numberProfile.AggregateValues["CountConsecutiveCalls"],2).ToString(),
	                                Math.Round(numberProfile.AggregateValues["CountInLowDurationCalls"],2).ToString(), 
		                            numberProfile.StrategyId.ToString()
                                 }
                   );




                }
                streamWriter.Close();
            }

            MySqlConnection mySqlConnection = new MySqlConnection(GetConnectionString());
            string query = String.Format(@"LOAD DATA LOCAL  INFILE '{0}' INTO TABLE [FraudAnalysis].NumberProfile FIELDS TERMINATED BY ',' LINES TERMINATED BY '\r'   ;", filename.Replace(@"\", @"\\"));
            mySqlConnection.Open();
            MySqlCommand mySqlCommand = new MySqlCommand(query, mySqlConnection);
            mySqlCommand.CommandTimeout = int.MaxValue;
            mySqlCommand.ExecuteNonQuery();
            mySqlConnection.Close();
        }

        public IEnumerable<FraudResult> GetFilteredSuspiciousNumbers(string tempTableKey, int fromRow, int toRow, DateTime fromDate, DateTime toDate, List<int> strategiesList, List<int> suspicionLevelsList)
        {
            throw new NotImplementedException();
        }

        public bool SaveSubscriberCase(SubscriberCase subscriberCaseObject)
        {
            throw new NotImplementedException();
        }

        public FraudResult GetFraudResult(DateTime fromDate, DateTime toDate, List<int> strategiesList, List<int> suspicionLevelsList, string subscriberNumber)
        {
            throw new NotImplementedException();
        }
    }
}
