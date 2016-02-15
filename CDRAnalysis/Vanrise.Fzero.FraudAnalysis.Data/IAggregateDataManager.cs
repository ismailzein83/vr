using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Aggregates;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IAggregateDataManager : IDataManager
    {
        List<AggregateDefinitionInfo> GetAggregates();

        bool AreAggregatesUpdated(ref object updateHandle);
    }
}
