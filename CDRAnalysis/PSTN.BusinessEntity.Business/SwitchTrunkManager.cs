using PSTN.BusinessEntity.Data;
using PSTN.BusinessEntity.Entities;

namespace PSTN.BusinessEntity.Business
{
    public class SwitchTrunkManager
    {
        public Vanrise.Entities.IDataRetrievalResult<SwitchTrunkDetail> GetFilteredSwitchTrunks(Vanrise.Entities.DataRetrievalInput<SwitchTrunkDetailQuery> input)
        {
            ISwitchTrunkDataManager dataManager = PSTNBEDataManagerFactory.GetDataManager<ISwitchTrunkDataManager>();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, dataManager.GetFilteredSwitchTrunks(input));
        }
    }
}
