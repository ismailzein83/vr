using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
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

        //public void SaveSuspiciousNumbers(List<SuspiciousNumber> suspiciousNumbers)
        //{

        //    StreamForBulkInsert stream = InitializeStreamForBulkInsert();

        //    foreach (SuspiciousNumber suspiciousNumber in suspiciousNumbers)
        //    {               

        //        string record=  string.Format("0,{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21}",                             
        //                         suspiciousNumber.DateDay.Value,
        //                         suspiciousNumber.Number,
        //                         GetDictionaryValue(suspiciousNumber.CriteriaValues, 0),
        //                         GetDictionaryValue(suspiciousNumber.CriteriaValues, 1),
        //                         GetDictionaryValue(suspiciousNumber.CriteriaValues, 2),
        //                         GetDictionaryValue(suspiciousNumber.CriteriaValues, 3),
        //                         GetDictionaryValue(suspiciousNumber.CriteriaValues, 4),
        //                         GetDictionaryValue(suspiciousNumber.CriteriaValues, 5), 
        //                         GetDictionaryValue(suspiciousNumber.CriteriaValues, 6),
        //                         GetDictionaryValue(suspiciousNumber.CriteriaValues, 7),
        //                         GetDictionaryValue(suspiciousNumber.CriteriaValues, 8),
        //                         GetDictionaryValue(suspiciousNumber.CriteriaValues, 9), 
        //                         GetDictionaryValue(suspiciousNumber.CriteriaValues, 10), 
        //                         GetDictionaryValue(suspiciousNumber.CriteriaValues, 11),
        //                         GetDictionaryValue(suspiciousNumber.CriteriaValues, 12),
        //                         GetDictionaryValue(suspiciousNumber.CriteriaValues, 13),
        //                         GetDictionaryValue(suspiciousNumber.CriteriaValues, 14),
        //                         GetDictionaryValue(suspiciousNumber.CriteriaValues, 15),
        //                         GetDictionaryValue(suspiciousNumber.CriteriaValues, 16),
        //                         GetDictionaryValue(suspiciousNumber.CriteriaValues, 17), 
        //                         suspiciousNumber.SuspectionLevel,
        //                         suspiciousNumber.StrategyId);

        //        stream.WriteRecord(record);

        //    }

        //    stream.Close();

        //    InsertBulkToTable(
        //        new StreamBulkInsertInfo
        //        {
        //            TableName = "[FraudAnalysis].[SubscriberThreshold]",
        //            Stream = stream,
        //            TabLock = false,
        //            KeepIdentity = false,
        //            FieldSeparator = ','
        //        });
        //}

        public object GetDictionaryValue<T>(Dictionary<T, Decimal> dictionary, T key)
        {
            Decimal value;
            if (!dictionary.TryGetValue(key, out value))
                return "";
            return Math.Round(value, 5);
        }

        //public void SaveNumberProfiles(List<NumberProfile> numberProfiles)
        //{

        //    StreamForBulkInsert stream = InitializeStreamForBulkInsert();

        //    foreach (NumberProfile numberProfile in numberProfiles)
        //    {

        //        stream.WriteRecord("0,{0},{1},{2},{3},{4},{5},{6},0,0,0,{7},0,0,{8},{9},{10},{11},{12},{13},{14},{15},{16},{17},{18},{19},{20},{21},{22},{23},{24},{25},{26},{27},{28}",
        //                            numberProfile.SubscriberNumber,
        //                            numberProfile.FromDate,
        //                            numberProfile.ToDate,
        //                            GetDictionaryValue(numberProfile.AggregateValues, "CountOutCalls"),
        //                            GetDictionaryValue(numberProfile.AggregateValues, "DiffOutputNumb"),
        //                            GetDictionaryValue(numberProfile.AggregateValues, "CountOutInters"),
        //                            GetDictionaryValue(numberProfile.AggregateValues, "CountInInters"),                                   
        //                            GetDictionaryValue(numberProfile.AggregateValues, "CallOutDurAvg"),                                    
        //                            GetDictionaryValue(numberProfile.AggregateValues, "CountOutFails"),
        //                            GetDictionaryValue(numberProfile.AggregateValues, "CountInFails"),
        //                            GetDictionaryValue(numberProfile.AggregateValues, "TotalOutVolume"),
        //                            GetDictionaryValue(numberProfile.AggregateValues, "TotalInVolume"),
        //                            GetDictionaryValue(numberProfile.AggregateValues, "DiffInputNumbers"),
        //                            GetDictionaryValue(numberProfile.AggregateValues, "CountOutSMSs"),
        //                            GetDictionaryValue(numberProfile.AggregateValues, "TotalIMEI"),
        //                            GetDictionaryValue(numberProfile.AggregateValues, "TotalBTS"),
        //                            numberProfile.IsOnNet,
        //                            GetDictionaryValue(numberProfile.AggregateValues, "TotalDataVolume"),
        //                            numberProfile.PeriodId,
        //                            GetDictionaryValue(numberProfile.AggregateValues, "CountInCalls"),
        //                            GetDictionaryValue(numberProfile.AggregateValues, "CallInDurAvg"),
        //                            GetDictionaryValue(numberProfile.AggregateValues, "CountOutOnNets"),
        //                            GetDictionaryValue(numberProfile.AggregateValues, "CountInOnNets"),
        //                            GetDictionaryValue(numberProfile.AggregateValues, "CountOutOffNets"),
        //                            GetDictionaryValue(numberProfile.AggregateValues, "CountInOffNets"),
        //                            GetDictionaryValue(numberProfile.AggregateValues, "CountFailConsecutiveCalls"),
        //                            GetDictionaryValue(numberProfile.AggregateValues, "CountConsecutiveCalls"),
        //                            GetDictionaryValue(numberProfile.AggregateValues, "CountInLowDurationCalls"),
        //                            numberProfile.StrategyId
        //       );




        //    }

        //    stream.Close();

        //    InsertBulkToTable(
        //        new StreamBulkInsertInfo
        //        {
        //            TableName = "[FraudAnalysis].[NumberProfile]",
        //            Stream = stream,
        //            TabLock = false,
        //            KeepIdentity = false,
        //            FieldSeparator = ','
        //        });
        //}


        public IEnumerable<FraudResult> GetFilteredSuspiciousNumbers(string tempTableKey, int fromRow, int toRow, DateTime fromDate, DateTime toDate, List<int> strategiesList, List<int> suspicionLevelsList, List<int> caseStatusesList)
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


            ExecuteNonQuerySP("[FraudAnalysis].[sp_FraudResult_CreateTempForFilteredSuspiciousNumbers]", tempTableName.TableName, fromDate, toDate, string.Join(",", strategiesList), string.Join(",", suspicionLevelsList), string.Join(",", caseStatusesList));
            int totalDataCount;
            rslt.Data = GetDataFromTempTable<FraudResult>(tempTableName.TableName, fromRow, toRow, "SubscriberNumber", false, FraudResultMapper, out totalDataCount);
            rslt.TotalCount = totalDataCount;
            return rslt.Data;
        }


        public FraudResult GetFraudResult(DateTime fromDate, DateTime toDate, List<int> strategiesList, List<int> suspicionLevelsList, string subscriberNumber)
        {
            return GetItemsSP("FraudAnalysis.sp_FraudResult_Get", FraudResultMapper, fromDate, toDate, string.Join(",", strategiesList), string.Join(",", suspicionLevelsList), subscriberNumber).FirstOrDefault();
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
            fraudResult.StatusId =  GetReaderValue<int?>(reader,"StatusId")  ;
            fraudResult.ValidTill = GetReaderValue<DateTime?>(reader, "ValidTill");
            return fraudResult;
        }




        public void UpdateSusbcriberCases(List<string> suspiciousNumbers)
        {
            DataTable dataTable = new DataTable("[FraudAnalysis].[SubscriberCaseType]");
            //we create column names as per the type in DB 
            dataTable.Columns.Add("SubscriberNumber", typeof(string));
            foreach(var i in suspiciousNumbers)
            {
                dataTable.Rows.Add(i);
            }



            ExecuteNonQuerySPCmd("[FraudAnalysis].[sp_FraudResult_UpdateSubscriberCases]",
                  (cmd) =>
                  {

                      SqlParameter parameter = new SqlParameter();
                      parameter.ParameterName = "@SubscriberCase";
                      parameter.SqlDbType = System.Data.SqlDbType.Structured;
                      parameter.Value = dataTable;
                      parameter.TypeName = "[FraudAnalysis].[SubscriberCaseType]";
                      cmd.Parameters.Add(parameter);
                  });
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[FraudAnalysis].[SubscriberThreshold]",
                Stream = streamForBulkInsert,
                TabLock = false,
                KeepIdentity = false,
                FieldSeparator = '^'
            };
        }

        public object InitialiazeStreamForDBApply()
        {
            return base.InitializeStreamForBulkInsert();
        }

        public void WriteRecordToStream(SuspiciousNumber record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("0^{0}^{1}^{2}^{3}^{4}",
                                 record.DateDay.Value,
                                 record.Number,
                                 record.SuspectionLevel,
                                 record.StrategyId,
                                 Vanrise.Common.Serializer.Serialize(record.CriteriaValues, true));
        }


        public void ApplySuspiciousNumbersToDB(object preparedSuspiciousNumbers)
        {
            InsertBulkToTable(preparedSuspiciousNumbers as BaseBulkInsertInfo);
        }
    }
}
