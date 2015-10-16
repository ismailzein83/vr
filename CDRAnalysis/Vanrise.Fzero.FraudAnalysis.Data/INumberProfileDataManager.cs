using Vanrise.Data;
using Vanrise.Entities;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Data
{
    public interface INumberProfileDataManager : IDataManager, IBulkApplyDataManager<NumberProfile>
    {
        void ApplyNumberProfilesToDB(object preparedNumberProfiles);

        BigResult<NumberProfile> GetNumberProfiles(Vanrise.Entities.DataRetrievalInput<NumberProfileQuery> input);
    }
}
