using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Data
{
    public interface ISwitchTrunkDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<SwitchTrunkDetail> GetFilteredSwitchTrunks(Vanrise.Entities.DataRetrievalInput<SwitchTrunkQuery> input);

        SwitchTrunkDetail GetSwitchTrunkByID(int trunkID);

        List<SwitchTrunkInfo> GetSwitchTrunksBySwitchID(int switchID);

        bool AddSwitchTrunk(SwitchTrunk trunkObject, out int insertedID);

        bool UpdateSwitchTrunk(SwitchTrunk trunkObject);

        bool DeleteSwitchTrunk(int trunkID);

        void UnlinkSwitchTrunks(int switchTrunkID, int linkedToTrunkID);

        void LinkSwitchTrunks(int switchTrunkID, int linkedToTrunkID);
    }
}
