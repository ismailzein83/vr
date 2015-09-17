using System;
using System.Collections.Generic;
using Vanrise.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IStrategyExecutionDataManager : IDataManager, IBulkApplyDataManager<StrategyExecutionDetail>
    {
        bool ExecuteStrategy(StrategyExecution strategyExecutionObject, out int insertedId);

        void ApplyStrategyExecutionDetailsToDB(object preparedStrategyExecutionDetails);

        bool OverrideStrategyExecution(int StrategyID, DateTime From, DateTime To);

        void DeleteStrategyExecutionDetails_StrategyExecutionID(int StrategyExecutionId);

        void LoadStrategyExecutionDetailSummaries(Action<StrategyExecutionDetailSummary> onBatchReady);

        List<int> GetCasesIDsofStrategyExecutionDetails(string accountNumber, DateTime? fromDate, DateTime? toDate, List<int> strategyIDs);

        void DeleteStrategyExecutionDetails_ByFilters(string accountNumber, DateTime? fromDate, DateTime? toDate, List<int> strategyIDs);

        
    }
}
