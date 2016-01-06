using System.Collections.Generic;
using Vanrise.Fzero.FraudAnalysis.Entities;
using System;
using Vanrise.Data;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IDWStrategyDataManager : IDataManager , IBulkApplyDataManager<DWStrategy>
    {
        List<DWStrategy> GetStrategies();

        void ApplyDWStrategiesToDB(object preparedDWStrategies);

        void SaveDWStrategiesToDB(List<DWStrategy> dwStrategies);
    }
}
