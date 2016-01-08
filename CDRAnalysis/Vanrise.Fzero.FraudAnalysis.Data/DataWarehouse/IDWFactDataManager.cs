using Vanrise.Data;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface IDWFactDataManager : IDataManager, IBulkApplyDataManager<DWFact>
    {
        void ApplyDWFactsToDB(object preparedDWFacts);
    }
}
