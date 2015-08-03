using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;


namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class NumberProfileDataManager : BaseSQLDataManager, INumberProfileDataManager
    {
        public NumberProfileDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public void ApplyNumberProfilesToDB(object preparedNumberProfiles)
        {
            InsertBulkToTable(preparedNumberProfiles as BaseBulkInsertInfo);
        }

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[FraudAnalysis].[NumberProfile]",
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

        public void WriteRecordToStream(NumberProfile record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("0^{0}^{1}^{2}^{3}^{4}^{5}",
                                    record.SubscriberNumber,
                                    record.FromDate,
                                    record.ToDate,
                                    record.PeriodId,
                                    record.StrategyId,
                                    Vanrise.Common.Serializer.Serialize(record.AggregateValues, true));
        }

        public BigResult<NumberProfile> GetNumberProfiles(Vanrise.Entities.DataRetrievalInput<NumberProfileResultQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_FraudResult_CreateTempForFilteredNumberProfiles", tempTableName, input.Query.FromDate, input.Query.ToDate, input.Query.SubscriberNumber);
            };
            return RetrieveData(input, createTempTableAction, NumberProfileMapper);
        }



        #region Private Methods
        private NumberProfile NumberProfileMapper(IDataReader reader)
        {
            var numberProfile = new NumberProfile();
            numberProfile.FromDate = (DateTime)reader["FromDate"];
            numberProfile.ToDate = (DateTime)reader["ToDate"];
            numberProfile.StrategyId = (int)reader["StrategyId"];
            numberProfile.PeriodId = (int)reader["PeriodId"];
            numberProfile.SubscriberNumber = reader["SubscriberNumber"] as string;
            numberProfile.AggregateValues = Vanrise.Common.Serializer.Deserialize<Dictionary<string, decimal>>(GetReaderValue<string>(reader, "AggregateValues"));

            return numberProfile;
        }
        #endregion
    }
}
