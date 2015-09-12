using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class StrategyExecutionDataManager : BaseSQLDataManager, IStrategyExecutionDataManager
    {

        public StrategyExecutionDataManager()
            : base("CDRDBConnectionString")
        {

        }

        public bool ExecuteStrategy(StrategyExecution strategyExecutionObject, out int insertedId)
        {
            object id;
            int recordesEffected = ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecution_Insert", out id,
                strategyExecutionObject.ProcessID,
                strategyExecutionObject.StrategyID,
                strategyExecutionObject.FromDate,
                strategyExecutionObject.ToDate,
                strategyExecutionObject.PeriodID,
                DateTime.Now
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

        public object FinishDBApplyStream(object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.Close();
            return new StreamBulkInsertInfo
            {
                TableName = "[FraudAnalysis].[StrategyExecutionDetails]",
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

        public void WriteRecordToStream(StrategyExecutionDetail record, object dbApplyStream)
        {
            StreamForBulkInsert streamForBulkInsert = dbApplyStream as StreamForBulkInsert;
            streamForBulkInsert.WriteRecord("0^{0}^{1}^{2}^{3}^{4}^{5}^{6}^{7}",
                                 record.StrategyExecutionID,
                                 record.AccountNumber,
                                 record.SuspicionLevelID,
                                 Vanrise.Common.Serializer.Serialize(record.FilterValues, true),
                                 Vanrise.Common.Serializer.Serialize(record.AggregateValues, true),
                                 null,
                                 (int)record.SuspicionOccuranceStatus,
                                 string.Join<string>(",", record.IMEIs)
                                 );
        }

        public void ApplyStrategyExecutionDetailsToDB(object preparedStrategyExecutionDetails)
        {
            InsertBulkToTable(preparedStrategyExecutionDetails as BaseBulkInsertInfo);
        }

        public bool OverrideStrategyExecution(int StrategyID, DateTime From, DateTime To, out int updatedId)
        {
            object id;
            int recordesEffected = ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecution_Override", out id,
                StrategyID, From, To
            );

            if (recordesEffected > 0)
            {
                updatedId = (int)id;
                return true;
            }
            else
            {
                updatedId = 0;
                return false;
            }
        }

        public void DeleteStrategyExecutionDetails(int StrategyExecutionId)
        {
            ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecutionDetails_Delete", StrategyExecutionId);
        }

        public void LoadAccountNumbersfromStrategyExecutionDetails(Action<StrategyExecutionDetail> onBatchReady)
        {
            ExecuteReaderSP("FraudAnalysis.sp_StrategyExecutionDetails_Load", (reader) =>
            {
                while (reader.Read())
                {
                    onBatchReady(new StrategyExecutionDetail() { AccountNumber = (reader["AccountNumber"] as string)   });
                }



            }
               );
        }




    }
}
