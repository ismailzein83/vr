using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Data
{
    public interface ISwitchTrunkDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<SwitchTrunkDetail> GetFilteredSwitchTrunks(Vanrise.Entities.DataRetrievalInput<SwitchTrunkQuery> input);

        SwitchTrunkDetail GetSwitchTrunkByID(int trunkID);

        SwitchTrunkInfo GetSwitchTrunkBySymbol(string symbol);

        List<SwitchTrunkInfo> GetSwitchTrunksBySwitchID(int switchID);

        List<SwitchTrunkInfo> GetSwitchTrunks();

        List<SwitchTrunkInfo> GetSwitchTrunksByIds(List<int> trunkIds);

        bool AddSwitchTrunk(SwitchTrunk trunkObject, out int insertedID);

        bool UpdateSwitchTrunk(SwitchTrunk trunkObject);

        bool DeleteSwitchTrunk(int trunkID);

        void UnlinkSwitchTrunk(int trunkID);

        void LinkSwitchTrunks(int switchTrunkID, int linkedToTrunkID);
    }
}
