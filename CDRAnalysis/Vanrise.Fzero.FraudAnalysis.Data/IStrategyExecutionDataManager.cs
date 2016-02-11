using System;
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

        void LoadStrategyExecutionItemSummaries(Action<StrategyExecutionItemSummary> onBatchReady);

        bool CancelStrategyExecution(long strategyExecutionId, int userId);

        StrategyExecution GetStrategyExecution(long strategyExecutionId);

    }
}
