using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data.SQL
{
    public class StrategyExecutionDataManager : BaseSQLDataManager, IStrategyExecutionDataManager
    {
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();

        static string[] s_Columns = new string[] {
            "ID"
            ,"StrategyExecutionID"  
            ,"AccountNumber"
            ,"SuspicionLevelID"
            ,"FilterValues"
            ,"AggregateValues" 
            ,"CaseID" 
            ,"SuspicionOccuranceStatus"  
            ,"IMEIs"  
        };



        public StrategyExecutionDataManager()
            : base("CDRDBConnectionString")
        {

        }


        static StrategyExecutionDataManager()
        {
            _columnMapper.Add("StrategyName", "StrategyID");
            _columnMapper.Add("PeriodName", "PeriodID");
            _columnMapper.Add("Entity.ExecutionDate", "ExecutionDate");
            _columnMapper.Add("Entity.FromDate", "FromDate");
            _columnMapper.Add("Entity.ToDate", "ToDate");
            _columnMapper.Add("ExecutedByName", "ExecutedBy");
            _columnMapper.Add("CancelledByName", "CancelledBy");
            _columnMapper.Add("Entity.CancellationDate", "CancellationDate");
            _columnMapper.Add("Entity.ExecutionDuration", "ExecutionDuration");
            _columnMapper.Add("Entity.NumberofCases", "NumberofCases");
            _columnMapper.Add("Entity.NumberofCDRs", "NumberofCDRs");
            _columnMapper.Add("Entity.NumberofSubscribers", "NumberofSubscribers");
            _columnMapper.Add("Entity.ID", "ID");
            _columnMapper.Add("StatusDescription", "Status");
        }

        public StrategyExecution GetStrategyExecution(long strategyExecutionId)
        {
            return GetItemSP("FraudAnalysis.sp_StrategyExecution_GetByID", StrategyExecutionMapper, strategyExecutionId);
        }

        public BigResult<StrategyExecution> GetFilteredStrategyExecutions(Vanrise.Entities.DataRetrievalInput<StrategyExecutionQuery> input)
        {
            string strategyIDs = (input.Query.StrategyIds != null && input.Query.StrategyIds.Count > 0) ? string.Join(",", input.Query.StrategyIds) : null;
            string userIDs = (input.Query.UserIds != null && input.Query.UserIds.Count > 0) ? string.Join(",", input.Query.UserIds) : null;
            string statusIDs = (input.Query.StatusIds != null && input.Query.StatusIds.Count > 0) ? string.Join(",", input.Query.StatusIds) : null;

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecution_CreateTempByFilters", tempTableName, input.Query.FromCDRDate, input.Query.ToCDRDate, input.Query.FromExecutionDate, input.Query.ToExecutionDate,
                    input.Query.FromCancellationDate, input.Query.ToCancellationDate, strategyIDs, input.Query.PeriodId, userIDs, statusIDs);
            };

            return RetrieveData(input, createTempTableAction, StrategyExecutionMapper, _columnMapper);
        }

        private StrategyExecution StrategyExecutionMapper(IDataReader reader)
        {
            var item = new StrategyExecution();
            item.ID = (long)reader["ID"];
            item.ProcessID = (long)reader["ProcessID"];
            item.StrategyID = (int)reader["StrategyID"];
            item.FromDate = (DateTime)reader["FromDate"];
            item.ToDate = (DateTime)reader["ToDate"];
            item.PeriodID = (int)reader["PeriodID"];
            item.ExecutionDate = (DateTime)reader["ExecutionDate"];
            item.CancellationDate = GetReaderValue<DateTime?>(reader, "CancellationDate");
            item.ExecutedBy = GetReaderValue<int>(reader, "ExecutedBy");
            item.CancelledBy = GetReaderValue<int?>(reader, "CancelledBy");
            item.NumberofSubscribers = GetReaderValue<int?>(reader, "NumberofSubscribers");
            item.NumberofCDRs = GetReaderValue<int?>(reader, "NumberofCDRs");
            item.NumberofCases = GetReaderValue<int?>(reader, "NumberofCases");
            item.ExecutionDuration = GetReaderValue<int?>(reader, "ExecutionDuration");
            item.Status = GetReaderValue<SuspicionOccuranceStatus>(reader, "Status");

            return item;
        }

        public bool ExecuteStrategy(StrategyExecution strategyExecutionObject, out int insertedId)
        {
            object id;
            int recordsEffected = ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecution_Insert", out id,
                strategyExecutionObject.ProcessID,
                strategyExecutionObject.StrategyID,
                strategyExecutionObject.FromDate,
                strategyExecutionObject.ToDate,
                strategyExecutionObject.PeriodID,
                DateTime.Now,
                strategyExecutionObject.ExecutedBy,
                strategyExecutionObject.Status
            );

            if (recordsEffected > 0)
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
                TableName = "[FraudAnalysis].[StrategyExecutionItem]",
                ColumnNames = s_Columns,
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

        public void WriteRecordToStream(StrategyExecutionItem record, object dbApplyStream)
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

        public void ApplyStrategyExecutionItemsToDB(object preparedStrategyExecutionItem)
        {
            InsertBulkToTable(preparedStrategyExecutionItem as BaseBulkInsertInfo);
        }

        public bool CancelStrategyExecution(long strategyExecutionId, int userId)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecution_Cancel", strategyExecutionId, userId);
            return (recordsAffected > 0);
        }

        public bool CloseStrategyExecution(long strategyExecutionId, long numberofSubscribers, long numberofCDRs, long numberofCases, long executionDuration)
        {
            int recordsAffected = ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecution_Close", strategyExecutionId, numberofSubscribers, numberofCDRs, numberofCases, executionDuration);

            return (recordsAffected > 0);
        }

        public void LoadStrategyExecutionItemSummaries(Action<StrategyExecutionItemSummary> onBatchReady)
        {
            ExecuteReaderSP("FraudAnalysis.sp_StrategyExecutionItem_GetByNULLCaseID", (reader) =>
            {
                while (reader.Read())
                {

                    HashSet<string> iMEIs = new HashSet<string>();

                    string IMEIs = GetReaderValue<string>(reader, "IMEIs");
                    if (IMEIs != null)
                        foreach (var i in IMEIs.Split(','))
                        {
                            iMEIs.Add(i);

                        }

                    onBatchReady(new StrategyExecutionItemSummary() { AccountNumber = (reader["AccountNumber"] as string), IMEIs = iMEIs });
                }



            }
               );
        }


        #region Private Members

        private int CaseMapper(IDataReader reader)
        {
            return (int)reader["CaseID"];
        }

        #endregion


    }
}
