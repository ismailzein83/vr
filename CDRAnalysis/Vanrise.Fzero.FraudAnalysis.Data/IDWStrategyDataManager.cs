using System.Collections.Generic;
using Vanrise.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IDWStrategyDataManager : IDataManager , IBulkApplyDataManager<DWStrategy>
    {
        List<DWStrategy> GetStrategies();

        void ApplyDWStrategiesToDB(object preparedDWStrategies);

        void SaveDWStrategiesToDB(List<DWStrategy> dwStrategies);
    }
}
