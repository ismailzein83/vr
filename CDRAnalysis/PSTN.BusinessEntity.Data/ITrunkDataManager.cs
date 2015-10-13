using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Data
{
    public interface ITrunkDataManager : IDataManager
    {
        Vanrise.Entities.BigResult<TrunkDetail> GetFilteredTrunks(Vanrise.Entities.DataRetrievalInput<TrunkQuery> input);

        TrunkDetail GetTrunkById(int trunkId);

        TrunkInfo GetTrunkBySymbol(string symbol);

        List<TrunkInfo> GetTrunksBySwitchIds(TrunkFilter trunkFilterObj);

        List<TrunkInfo> GetTrunks();

        List<TrunkInfo> GetTrunksByIds(List<int> trunkIds);

        bool AddTrunk(Trunk trunkObj, out int insertedId);

        bool UpdateTrunk(Trunk trunkObj);

        bool DeleteTrunk(int trunkId);

        void UnlinkTrunk(int trunkId);

        void LinkTrunks(int switchTrunkId, int linkedToTrunkId);
    }
}
