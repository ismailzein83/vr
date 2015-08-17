﻿using System;
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


        public BigResult<FraudResult> GetFilteredSuspiciousNumbers(Vanrise.Entities.DataRetrievalInput<FraudResultQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("[FraudAnalysis].[sp_FraudResult_CreateTempForFilteredSuspiciousNumbers]", tempTableName, input.Query.FromDate, input.Query.ToDate, input.Query.StrategiesList, input.Query.SuspicionLevelsList, input.Query.CaseStatusesList);
            };
            return RetrieveData(input, createTempTableAction, FraudResultMapper);
        }

        public FraudResult GetFraudResult(DateTime fromDate, DateTime toDate, List<int> strategiesList, List<int> suspicionLevelsList, string accountNumber)
        {
            return GetItemsSP("FraudAnalysis.sp_FraudResult_Get", FraudResultMapper, fromDate, toDate, string.Join(",", strategiesList), string.Join(",", suspicionLevelsList), accountNumber).FirstOrDefault();
        }


        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[FraudAnalysis].[AccountThreshold]",
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


        public BigResult<AccountThreshold> GetAccountThresholds(Vanrise.Entities.DataRetrievalInput<AccountThresholdResultQuery> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_FraudResult_CreateTempForFilteredAccountThresholds", tempTableName, input.Query.FromDate, input.Query.ToDate, input.Query.AccountNumber);
            };
            return RetrieveData(input, createTempTableAction, AccountThresholdMapper);
        }



        #region Private Methods

        private FraudResult FraudResultMapper(IDataReader reader)
        {
            var fraudResult = new FraudResult();
            fraudResult.LastOccurance = (DateTime)reader["LastOccurance"];
            fraudResult.AccountNumber = reader["AccountNumber"] as string;
            fraudResult.SuspicionLevelName = ((Enums.SuspicionLevel)Enum.ToObject(typeof(Enums.SuspicionLevel), GetReaderValue<int>(reader, "SuspicionLevelId"))).ToString();
            fraudResult.StrategyName = reader["StrategyName"] as string;
            fraudResult.NumberofOccurances = (int)reader["NumberofOccurances"];
            fraudResult.CaseStatus = reader["CaseStatus"] as string;
            fraudResult.StatusId = GetReaderValue<int?>(reader, "StatusId");
            fraudResult.ValidTill = GetReaderValue<DateTime?>(reader, "ValidTill");
            return fraudResult;
        }

        private AccountThreshold AccountThresholdMapper(IDataReader reader)
        {
            var accountThreshold = new AccountThreshold();

            accountThreshold.DateDay = GetReaderValue<DateTime>(reader, "DateDay");
            accountThreshold.SuspicionLevelName = reader["SuspicionLevelName"] as string;
            accountThreshold.StrategyName = reader["StrategyName"] as string;
            accountThreshold.CriteriaValues = Vanrise.Common.Serializer.Deserialize<Dictionary<int, decimal>>(GetReaderValue<string>(reader, "CriteriaValues"));

            return accountThreshold;
        }

        #endregion

      

        
    }
}
