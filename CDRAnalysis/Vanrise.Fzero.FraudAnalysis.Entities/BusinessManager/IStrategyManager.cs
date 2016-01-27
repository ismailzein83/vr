using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Fzero.FraudAnalysis.Entities
{
    public interface IStrategyManager : IBusinessManager
    {
        int? GetStrategyPeriodId(int strategyId);
        IEnumerable<string> GetStrategyNames(IEnumerable<int> strategyIds);
    }
}
