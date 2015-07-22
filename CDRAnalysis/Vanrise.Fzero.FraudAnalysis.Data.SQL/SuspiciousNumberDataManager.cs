using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class SuspiciousNumberDataManager : BaseSQLDataManager, ISuspiciousNumberDataManager
    {
        public SuspiciousNumberDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public void SaveSuspiciousNumbers(List<SuspiciousNumber> suspiciousNumbers)
        {

            StreamForBulkInsert stream = InitializeStreamForBulkInsert();

            foreach (SuspiciousNumber suspiciousNumber in suspiciousNumbers)
            {
                List<string> sValues = new List<string>();

                for (int i = 1; i <= 18; i++)
                {
                    if (suspiciousNumber.CriteriaValues.Where(x => x.Key == i).Count() == 1)
                    {
                        sValues.Add(Math.Round(suspiciousNumber.CriteriaValues.Where(x => x.Key == i).FirstOrDefault().Value, 2).ToString());
                    }
                    else
                    {
                        sValues.Add("");
                    }
                }

                string record=  string.Format("0,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21}",
                             new[] { 
                                 suspiciousNumber.DateDay.Value.ToString(),
                                 suspiciousNumber.Number.ToString(),
                                 sValues[0],
                                 sValues[1],
                                 sValues[2],
                                 sValues[3],
                                 sValues[4],
                                 sValues[5], 
                                 sValues[6],
                                 sValues[7],
                                 sValues[8],
                                 sValues[9], 
                                 sValues[10], 
                                 sValues[11],
                                 sValues[12],
                                 sValues[13],
                                 sValues[14],
                                 sValues[15],
                                 sValues[16],
                                 sValues[17], 
                                 suspiciousNumber.SuspectionLevel.ToString(),
                                 suspiciousNumber.StrategyId.ToString() });

                stream.WriteRecord(record);

            }

            stream.Close();

            InsertBulkToTable(
                new StreamBulkInsertInfo
                {
                    TableName = "[FraudAnalysis].[SubscriberThreshold]",
                    Stream = stream,
                    TabLock = false,
                    KeepIdentity = false,
                    FieldSeparator = ','
                });
        }

        public void SaveNumberProfiles(List<NumberProfile> numberProfiles)
        {

            StreamForBulkInsert stream = InitializeStreamForBulkInsert();

            foreach (NumberProfile numberProfile in numberProfiles)
            {

                stream.WriteRecord("0,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28},{29},{30},{31},{32},{33}",
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

            stream.Close();

            InsertBulkToTable(
                new StreamBulkInsertInfo
                {
                    TableName = "[FraudAnalysis].[NumberProfile]",
                    Stream = stream,
                    TabLock = false,
                    KeepIdentity = false,
                    FieldSeparator = ','
                });
        }


        public IEnumerable<FraudResult> GetFilteredSuspiciousNumbers(string tempTableKey, int fromRow, int toRow, DateTime fromDate, DateTime toDate, List<int> strategiesList, List<int> suspicionLevelsList)
        {

            TempTableName tempTableName = null;
            if (tempTableKey != null)
                tempTableName = GetTempTableName(tempTableKey);
            else
                tempTableName = GenerateTempTableName();


            BigResult<FraudResult> rslt = new BigResult<FraudResult>()
            {
                ResultKey = tempTableName.Key
            };


            ExecuteNonQuerySP("[FraudAnalysis].[sp_FraudResult_CreateTempForFilteredSuspiciousNumbers]", tempTableName.TableName, fromDate, toDate, string.Join(",", strategiesList), string.Join(",",suspicionLevelsList));
            int totalDataCount;
            rslt.Data = GetDataFromTempTable<FraudResult>(tempTableName.TableName, fromRow, toRow, "SubscriberNumber", false, FraudResultMapper, out totalDataCount);
            rslt.TotalCount = totalDataCount;
            return rslt.Data;
        }

        private FraudResult FraudResultMapper(IDataReader reader)
        {
            var fraudResult = new FraudResult();
            fraudResult.LastOccurance = (DateTime)reader["LastOccurance"];
            fraudResult.SubscriberNumber = reader["SubscriberNumber"] as string;
            fraudResult.SuspicionLevelName = ((Enums.SuspicionLevel)Enum.ToObject(typeof(Enums.SuspicionLevel), GetReaderValue<int>(reader, "SuspicionLevelId"))).ToString();
            fraudResult.StrategyName = reader["StrategyName"] as string;
            fraudResult.NumberofOccurances = (int)reader["NumberofOccurances"];
            fraudResult.CaseStatus = reader["CaseStatus"] as string;
            return fraudResult;
        }


        public bool SaveSubscriberCase(Strategy strategyObject)
        {

            //int recordesEffected = ExecuteNonQuerySP("FraudAnalysis.sp_Strategy_Update",
            //    strategyObject.Id,
            //     DefaultUserId,
            //    !string.IsNullOrEmpty(strategyObject.Name) ? strategyObject.Name : null,
            //    !string.IsNullOrEmpty(strategyObject.Description) ? strategyObject.Description : null,
            //    DateTime.Now,
            //    strategyObject.IsDefault,
            //    Vanrise.Common.Serializer.Serialize(strategyObject));
            //if (recordesEffected > 0)
            //    return true;
            return false;
        }

    }
}
