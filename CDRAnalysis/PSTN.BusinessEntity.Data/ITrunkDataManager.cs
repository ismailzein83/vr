using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;

namespace PSTN.BusinessEntity.Data
{
    public interface ITrunkDataManager : IDataManager
    {
        IEnumerable<Trunk> GetTrunks();
       
        bool AddTrunk(Trunk trunkObj, out int insertedId);

        bool UpdateTrunk(Trunk trunkObj);

        bool DeleteTrunk(int trunkId);

        void UnlinkTrunk(int trunkId);

        void LinkTrunks(int trunkId, int linkedToTrunkId);

        bool AreTrunksUpdated(ref object updateHandle);
        
    }
}
