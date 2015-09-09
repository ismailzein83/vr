using Vanrise.Fzero.FraudAnalysis.Data;
using Vanrise.Fzero.FraudAnalysis.Entities;

namespace Vanrise.Fzero.FraudAnalysis.Business
{
    public class NumberProfileManager
    {
        public Vanrise.Entities.IDataRetrievalResult<NumberProfile> GetNumberProfiles(Vanrise.Entities.DataRetrievalInput<NumberProfileResultQuery> input)
        {
            INumberProfileDataManager manager = FraudDataManagerFactory.GetDataManager<INumberProfileDataManager>();

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, manager.GetNumberProfiles(input));
        }
    }
}
