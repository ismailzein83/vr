using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IStrategyExecutionDataManager : IDataManager
    {
        BigResult<StrategyExecution> GetFilteredStrategyExecutions(Vanrise.Entities.DataRetrievalInput<StrategyExecutionQuery> input);

        bool AddStrategyExecution(StrategyExecution strategyExecutionObject, out int insertedId);

        bool CancelStrategyExecution(long strategyExecutionId, int userId);

        bool CloseStrategyExecution(long strategyExecutionId, long numberofSubscribers, long numberofCDRs, long numberofSuspicions, long executionDuration);

        StrategyExecution GetStrategyExecution(long strategyExecutionId);

    }
}
