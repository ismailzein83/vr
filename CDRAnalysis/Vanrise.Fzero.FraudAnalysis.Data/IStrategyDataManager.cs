using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IStrategyDataManager : IDataManager
    {
        List<Strategy> GetStrategies();

        bool AddStrategy(Strategy strategyObject, out int insertedId);

        bool UpdateStrategy(Strategy strategy);

        bool AreStrategiesUpdated(ref object updateHandle);
    }
}
