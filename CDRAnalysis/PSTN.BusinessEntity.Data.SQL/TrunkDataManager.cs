using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PSTN.BusinessEntity.Data.SQL
{
    public class TrunkDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ITrunkDataManager
    {
        private Dictionary<string, string> _mapper;

        public TrunkDataManager()
            : base("CDRDBConnectionString")
        {
            _mapper = new Dictionary<string, string>();

            _mapper.Add("TypeDescription", "Type");
            _mapper.Add("DirectionDescription", "Direction");
        }


        public IEnumerable<Trunk> GetTrunks()
        {
            return GetItemsSP("PSTN_BE.sp_SwitchTrunk_GetAll", TrunkMapper);
        }
     
        public bool AddTrunk(Trunk trunkObj, out int insertedId)
        {
            object trunkId;

            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_SwitchTrunk_Insert", out trunkId, trunkObj.Name, trunkObj.Symbol, trunkObj.SwitchId, trunkObj.Type, trunkObj.Direction);

            insertedId = (recordsAffected > 0) ? (int)trunkId : -1;
            return (recordsAffected > 0);
        }

        public bool UpdateTrunk(Trunk trunkObj)
        {
            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_SwitchTrunk_Update", trunkObj.TrunkId, trunkObj.Name, trunkObj.Symbol, trunkObj.SwitchId, trunkObj.Type, trunkObj.Direction);

            return (recordsAffected > 0);
        }

        public bool DeleteTrunk(int trunkId)
        {
            int recordsEffected = ExecuteNonQuerySP("PSTN_BE.sp_SwitchTrunk_Delete", trunkId);
            return (recordsEffected > 0);
        }

        public void UnlinkTrunk(int trunkId)
        {
            ExecuteNonQuerySP("PSTN_BE.sp_SwitchTrunk_Unlink", trunkId);
        }

        public void LinkTrunks(int trunkId, int linkedToTrunkId)
        {
            ExecuteNonQuerySP("PSTN_BE.sp_SwitchTrunk_LinkToTrunk", trunkId, linkedToTrunkId);
        }

        public bool AreTrunksUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("PSTN_BE.SwitchTrunk", ref updateHandle);
        }

        #region Mappers

        



        Trunk TrunkMapper(IDataReader reader)
        {
            Trunk trunk = new Trunk();

            trunk.TrunkId = (int)reader["ID"];
            trunk.Name = reader["Name"] as string;
            trunk.Symbol = reader["Symbol"] as string;
            trunk.SwitchId = (int)reader["SwitchID"];
            trunk.Type = (TrunkType)reader["Type"];
            trunk.Direction = (TrunkDirection)reader["Direction"];
            trunk.LinkedToTrunkId = GetReaderValue<int?>(reader, "LinkedToTrunkID");

            return trunk;
        }


        #endregion
    }
}
