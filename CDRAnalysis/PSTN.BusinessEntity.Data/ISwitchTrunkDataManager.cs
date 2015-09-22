using PSTN.BusinessEntity.Entities;

namespace PSTN.BusinessEntity.Data
{
    public interface ISwitchTrunkDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<SwitchTrunkDetail> GetFilteredSwitchTrunks(Vanrise.Entities.DataRetrievalInput<SwitchTrunkDetailQuery> input);
    }
}
