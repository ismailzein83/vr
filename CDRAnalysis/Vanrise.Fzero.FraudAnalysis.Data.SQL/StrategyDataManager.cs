using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class StrategyDataManager : BaseSQLDataManager, IStrategyDataManager
    {

        int DefaultUserId = 1;

        public StrategyDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public Strategy GetStrategy(int strategyId)
        {
            return GetItemsSP("FraudAnalysis.sp_Strategy_GetStrategy", StrategyMapper, strategyId).FirstOrDefault();
        }

        public List<Strategy> GetAllStrategies()
        {
            return GetItemsSP("FraudAnalysis.sp_Strategy_GetAll", StrategyMapper);
        }


        public List<SubscriberThreshold> GetSubscriberThresholds(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string msisdn)
        {
            return GetItemsSP("FraudAnalysis.sp_FraudResult_GetSubscriberThresholds", SubscriberThresholdMapper, fromRow, toRow, fromDate, toDate, msisdn);
        }

        public List<CDR> GetNormalCDRs(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string msisdn)
        {
            return GetItemsSP("FraudAnalysis.sp_FraudResult_GetNormalCDR", CDRMapper, fromRow, toRow, fromDate, toDate, msisdn);
        }


        public List<NumberProfile> GetNumberProfiles(int fromRow, int toRow, DateTime fromDate, DateTime toDate, string subscriberNumber)
        {
            return GetItemsSP("FraudAnalysis.sp_FraudResult_GetNumberProfile", NumberProfileMapper, fromRow, toRow, fromDate, toDate, subscriberNumber);
        }




        public List<Strategy> GetFilteredStrategies(int fromRow, int toRow, string name, string description)
        {
            return GetItemsSP("FraudAnalysis.sp_Strategy_GetFilteredStrategies", StrategyMapper, fromRow, toRow, name, description);
        }

        public bool AddStrategy(Strategy strategyObject, out int insertedId)
        {
            object id;
            int recordesEffected = ExecuteNonQuerySP("FraudAnalysis.sp_Strategy_Insert", out id,
                DefaultUserId,
                !string.IsNullOrEmpty(strategyObject.Name) ? strategyObject.Name : null,
                !string.IsNullOrEmpty(strategyObject.Description) ? strategyObject.Description : null,
                DateTime.Now,
                strategyObject.IsDefault,
                Vanrise.Common.Serializer.Serialize(strategyObject)

            );

            if (recordesEffected > 0)
            {
                insertedId = (int)id;
                return true;
            }
            else
            {
                insertedId = 0;
                return false;
            }


        }

        public bool UpdateStrategy(Strategy strategyObject)
        {

            int recordesEffected = ExecuteNonQuerySP("FraudAnalysis.sp_Strategy_Update",
                strategyObject.Id,
                 DefaultUserId,
                !string.IsNullOrEmpty(strategyObject.Name) ? strategyObject.Name : null,
                !string.IsNullOrEmpty(strategyObject.Description) ? strategyObject.Description : null,
                DateTime.Now,
                strategyObject.IsDefault,
                Vanrise.Common.Serializer.Serialize(strategyObject));
            if (recordesEffected > 0)
                return true;
            return false;
        }

        #region Private Methods


        private SubscriberThreshold SubscriberThresholdMapper(IDataReader reader)
        {
            var subscriberThreshold = new SubscriberThreshold();

            subscriberThreshold.DateDay = GetReaderValue<DateTime>(reader, "DateDay");
            subscriberThreshold.Criteria1 = GetReaderValue<Decimal>(reader, "Criteria1");
            subscriberThreshold.Criteria2 = GetReaderValue<Decimal>(reader, "Criteria2");
            subscriberThreshold.Criteria3 = GetReaderValue<Decimal>(reader, "Criteria3");
            subscriberThreshold.Criteria4 = GetReaderValue<Decimal>(reader, "Criteria4");
            subscriberThreshold.Criteria5 = GetReaderValue<Decimal>(reader, "Criteria5");
            subscriberThreshold.Criteria6 = GetReaderValue<Decimal>(reader, "Criteria6");
            subscriberThreshold.Criteria7 = GetReaderValue<Decimal>(reader, "Criteria7");
            subscriberThreshold.Criteria8 = GetReaderValue<Decimal>(reader, "Criteria8");
            subscriberThreshold.Criteria9 = GetReaderValue<Decimal>(reader, "Criteria9");
            subscriberThreshold.Criteria10 = GetReaderValue<Decimal>(reader, "Criteria10");
            subscriberThreshold.Criteria11 = GetReaderValue<Decimal>(reader, "Criteria11");
            subscriberThreshold.Criteria12 = GetReaderValue<Decimal>(reader, "Criteria12");
            subscriberThreshold.Criteria13 = GetReaderValue<Decimal>(reader, "Criteria13");
            subscriberThreshold.Criteria14 = GetReaderValue<Decimal>(reader, "Criteria14");
            subscriberThreshold.Criteria15 = GetReaderValue<Decimal>(reader, "Criteria15");
            subscriberThreshold.Criteria16 = GetReaderValue<Decimal>(reader, "Criteria16");
            subscriberThreshold.Criteria17 = GetReaderValue<Decimal>(reader, "Criteria17");
            subscriberThreshold.Criteria18 = GetReaderValue<Decimal>(reader, "Criteria18");
            subscriberThreshold.SuspicionLevelName = reader["SuspicionLevelName"] as string; 
            subscriberThreshold.StrategyName = reader["StrategyName"] as string; 

            return subscriberThreshold;
        }


        private CDR CDRMapper(IDataReader reader)
        {
            var normalCDR = new CDR();
            normalCDR.CallType = (Enums.CallType)Enum.ToObject(typeof(Enums.CallType), GetReaderValue<int>(reader, "Call_Type"));
            normalCDR.ConnectDateTime = GetReaderValue<DateTime?>(reader, "ConnectDateTime");
            normalCDR.IMSI = reader["IMSI"] as string;
            normalCDR.DurationInSeconds = GetReaderValue<Decimal?>(reader, "DurationInSeconds");
            normalCDR.CallClass = reader["Call_Class"] as string;
            normalCDR.SubType = reader["Sub_Type"] as string;
            normalCDR.IMEI = reader["IMEI"] as string;
            normalCDR.CellId = reader["Cell_Id"] as string;
            normalCDR.UpVolume = GetReaderValue<Decimal?>(reader, "Up_Volume");
            normalCDR.DownVolume = GetReaderValue<Decimal?>(reader, "Down_Volume");
            normalCDR.ServiceType = GetReaderValue<int>(reader, "Service_Type");
            normalCDR.ServiceVASName = reader["Service_VAS_Name"] as string;
            normalCDR.Destination = reader["Destination"] as string;
            return normalCDR;
        }

        private NumberProfile NumberProfileMapper(IDataReader reader)
        {
            var numberProfile = new NumberProfile();
            numberProfile.FromDate = (DateTime)reader["FromDate"];
            numberProfile.ToDate = (DateTime)reader["ToDate"];
            numberProfile.CountOutCalls = GetReaderValue<decimal?>(reader, "Count_Out_Calls");
            numberProfile.DiffOutputNumb = GetReaderValue<decimal?>(reader, "Diff_Output_Numb");
            numberProfile.CountOutInter = GetReaderValue<decimal?>(reader, "Count_Out_Inter");
            numberProfile.CountInInter = GetReaderValue<decimal?>(reader, "Count_In_Inter");
            numberProfile.CallOutDurAvg = GetReaderValue<decimal?>(reader, "Call_Out_Dur_Avg");
            numberProfile.CountOutFail = GetReaderValue<decimal?>(reader, "Count_Out_Fail");
            numberProfile.CountInFail = GetReaderValue<decimal?>(reader, "Count_In_Fail");
            numberProfile.TotalOutVolume = GetReaderValue<decimal?>(reader, "Total_Out_Volume");
            numberProfile.TotalInVolume = GetReaderValue<decimal?>(reader, "Total_In_Volume");
            numberProfile.DiffInputNumbers = GetReaderValue<decimal?>(reader, "Diff_Input_Numbers");
            numberProfile.CountOutSMS = GetReaderValue<decimal?>(reader, "Count_Out_SMS");
            numberProfile.TotalIMEI = GetReaderValue<decimal?>(reader, "Total_IMEI");
            numberProfile.TotalBTS = GetReaderValue<decimal?>(reader, "Total_BTS");
            numberProfile.TotalDataVolume = GetReaderValue<decimal?>(reader, "Total_Data_Volume");
            numberProfile.CountInCalls = GetReaderValue<decimal?>(reader, "Count_In_Calls");
            numberProfile.CallInDurAvg = GetReaderValue<decimal?>(reader, "Call_In_Dur_Avg");
            numberProfile.CountOutOnNet = GetReaderValue<decimal?>(reader, "Count_Out_OnNet");
            numberProfile.CountInOnNet = GetReaderValue<decimal?>(reader, "Count_In_OnNet");
            numberProfile.CountOutOffNet = GetReaderValue<decimal?>(reader, "Count_Out_OffNet");
            numberProfile.CountInOffNet = GetReaderValue<decimal?>(reader, "Count_In_OffNet");
            numberProfile.CountFailConsecutiveCalls = GetReaderValue<decimal?>(reader, "CountFailConsecutiveCalls");
            numberProfile.CountConsecutiveCalls = GetReaderValue<decimal?>(reader, "CountConsecutiveCalls");
            numberProfile.CountInLowDurationCalls = GetReaderValue<decimal?>(reader, "CountInLowDurationCalls");
            return numberProfile;
        }


        private Strategy StrategyMapper(IDataReader reader)
        {
            var strategy = Vanrise.Common.Serializer.Deserialize<Strategy>(GetReaderValue<string>(reader, "StrategyContent"));
            strategy.Id = (int)reader["Id"];
            return strategy;
        }

        private CallClass CallClassMapper(IDataReader reader)
        {
            var callClass = new CallClass();
            callClass.Id = (int)reader["Id"];
            callClass.Description = reader["Description"] as string;
            callClass.NetType = (Enums.NetType)Enum.ToObject(typeof(Enums.NetType), GetReaderValue<int>(reader, "NetType"));
            return callClass;
        }

        public List<CallClass> GetAllCallClasses()
        {
            return GetItemsSP("FraudAnalysis.sp_CallClass_GetAll", CallClassMapper);
        }

        public List<Period> GetPeriods()
        {

            var enumerationType = typeof(Enums.Period);
            List<Period> periods = new List<Period>();

            foreach (int value in Enum.GetValues(enumerationType))
            {
                var name = Enum.GetName(enumerationType, value);
                periods.Add(new Period() { Id = value, Name = name });
            }

            return periods;
        }


        #endregion



    }
}
