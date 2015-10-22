using System;
using System.Collections.Generic;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IFilterDataManager : IDataManager
    {
        List<FilterDefinition> GetFilters();

        bool AreFiltersUpdated(ref object updateHandle);
    }
}
