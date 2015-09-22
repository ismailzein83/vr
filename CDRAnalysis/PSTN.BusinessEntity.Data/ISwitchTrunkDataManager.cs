using PSTN.BusinessEntity.Entities;

namespace PSTN.BusinessEntity.Data
{
    public interface ISwitchTrunkDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<SwitchTrunkDetail> GetFilteredSwitchTrunks(Vanrise.Entities.DataRetrievalInput<SwitchTrunkDetailQuery> input);

        SwitchTrunkDetail GetSwitchTrunkByID(int trunkID);

        bool AddSwitchTrunk(SwitchTrunk trunkObject, out int insertedID);

        bool UpdateSwitchTrunk(SwitchTrunk trunkObject);
    }
}
