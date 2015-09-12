using System;
using Vanrise.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IStrategyExecutionDataManager : IDataManager, IBulkApplyDataManager<StrategyExecutionDetail>
    {
        bool ExecuteStrategy(StrategyExecution strategyExecutionObject, out int insertedId);

        void ApplyStrategyExecutionDetailsToDB(object preparedStrategyExecutionDetails);

        bool OverrideStrategyExecution(int StrategyID, DateTime From, DateTime To, out int updatedId);

        void DeleteStrategyExecutionDetails(int StrategyExecutionId);

        void LoadAccountNumbersfromStrategyExecutionDetails(Action<StrategyExecutionDetailSummary> onBatchReady);
    }
}
