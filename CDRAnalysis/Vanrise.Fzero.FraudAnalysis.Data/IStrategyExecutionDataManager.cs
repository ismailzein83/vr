using System;
using System.Collections.Generic;
using Vanrise.Data;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IStrategyExecutionDataManager : IDataManager, IBulkApplyDataManager<StrategyExecutionItem>
    {
        BigResult<StrategyExecution> GetFilteredStrategyExecutions(Vanrise.Entities.DataRetrievalInput<StrategyExecutionQuery> input);

        bool ExecuteStrategy(StrategyExecution strategyExecutionObject, out int insertedId);

        void ApplyStrategyExecutionItemsToDB(object preparedStrategyExecutionItems);

        bool OverrideStrategyExecution(int StrategyID, DateTime From, DateTime To);

        void DeleteStrategyExecutionItem_StrategyExecutionID(int StrategyExecutionId);

        void LoadStrategyExecutionItemSummaries(Action<StrategyExecutionItemSummary> onBatchReady);

        List<int> GetCasesIDsofStrategyExecutionItem(string accountNumber, DateTime? fromDate, DateTime? toDate, List<int> strategyIDs);

        void DeleteStrategyExecutionItem_ByFilters(string accountNumber, DateTime? fromDate, DateTime? toDate, List<int> strategyIDs);

        void DeleteStrategyExecutionItem_ByCaseIDs(List<int> caseIds);

    }
}
