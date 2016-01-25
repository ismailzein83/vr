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
        }



        public BigResult<StrategyExecution> GetFilteredStrategyExecutions(Vanrise.Entities.DataRetrievalInput<StrategyExecutionQuery> input)
        {
            string strategyIDs = (input.Query.StrategyIds != null && input.Query.StrategyIds.Count > 0) ? string.Join(",", input.Query.StrategyIds) : null;
            string periodIDs = (input.Query.PeriodIDs != null && input.Query.PeriodIDs.Count > 0) ? string.Join(",", input.Query.PeriodIDs) : null;

            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecution_CreateTempByFilters", tempTableName, input.Query.FromCDRDate, input.Query.ToCDRDate, input.Query.FromExecutionDate, input.Query.ToExecutionDate, strategyIDs, periodIDs);
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
                DateTime.Now
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
                TableName = "[FraudAnalysis].[StrategyExecutionDetails]",
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

        public bool OverrideStrategyExecution(int StrategyID, DateTime From, DateTime To)
        {
            int recordsEffected = ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecution_Override", StrategyID, From, To);

            return (recordsEffected > 0);
        }

        public void DeleteStrategyExecutionDetails_StrategyExecutionID(int StrategyExecutionId)
        {
            ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecutionDetails_DeleteByStrategyExecutionID", StrategyExecutionId);
        }

        public void LoadStrategyExecutionDetailSummaries(Action<StrategyExecutionDetailSummary> onBatchReady)
        {
            ExecuteReaderSP("FraudAnalysis.sp_StrategyExecutionDetails_GetByNULLCaseID", (reader) =>
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

                    onBatchReady(new StrategyExecutionDetailSummary() { AccountNumber = (reader["AccountNumber"] as string), IMEIs = iMEIs });
                }



            }
               );
        }


        public List<int> GetCasesIDsofStrategyExecutionDetails(string accountNumber, DateTime? fromDate, DateTime? toDate, List<int> strategyIDs)
        {
            string StrategiesCommaSeperatedList = null;

            if (strategyIDs != null)
                StrategiesCommaSeperatedList = string.Join(",", strategyIDs);

            return GetItemsSP("FraudAnalysis.sp_StrategyExecutionDetails_GetCaseIDs", CaseMapper, accountNumber, fromDate, toDate, StrategiesCommaSeperatedList);
        }


        public void DeleteStrategyExecutionDetails_ByFilters(string accountNumber, DateTime? fromDate, DateTime? toDate, List<int> strategyIDs)
        {
            string StrategiesCommaSeperatedList = null;

            if (strategyIDs != null)
                StrategiesCommaSeperatedList = string.Join(",", strategyIDs);

            ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecutionDetails_DeleteByFilters", accountNumber, fromDate, toDate, StrategiesCommaSeperatedList);
        }

        public void DeleteStrategyExecutionDetails_ByCaseIDs(List<int> caseIds)
        {
            string CasesCommaSeperatedList = null;

            if (caseIds != null)
                CasesCommaSeperatedList = string.Join(",", caseIds);

            ExecuteNonQuerySP("FraudAnalysis.sp_StrategyExecutionDetails_DeleteByCaseIDs", CasesCommaSeperatedList);
        }


        #region Private Members

        private int CaseMapper(IDataReader reader)
        {
            return (int)reader["CaseID"];
        }

        #endregion


    }
}
