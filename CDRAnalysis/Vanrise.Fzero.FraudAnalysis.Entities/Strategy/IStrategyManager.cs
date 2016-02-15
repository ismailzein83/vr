using System.Collections.Generic;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public interface IStrategyManager : IBusinessManager
    {
        IEnumerable<string> GetStrategyNames(IEnumerable<int> strategyIds);
    }
}
