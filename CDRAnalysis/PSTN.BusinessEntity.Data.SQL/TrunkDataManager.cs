using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PSTN.BusinessEntity.Data.SQL
{
    public class TrunkDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ITrunkDataManager
    {
        private Dictionary<string, string> _mapper;

        public TrunkDataManager() : base("CDRDBConnectionString") {
            _mapper = new Dictionary<string, string>();

            _mapper.Add("TypeDescription", "Type");
            _mapper.Add("DirectionDescription", "Direction");
        }

        public Vanrise.Entities.BigResult<TrunkDetail> GetFilteredTrunks(Vanrise.Entities.DataRetrievalInput<TrunkQuery> input)
        {
            return RetrieveData(input, (tempTableName) =>
            {
                string switchIDs = (input.Query.SelectedSwitchIds != null && input.Query.SelectedSwitchIds.Count() > 0) ?
                    string.Join<int>(",", input.Query.SelectedSwitchIds) : null;

                string types = (input.Query.SelectedTypes != null && input.Query.SelectedTypes.Count > 0) ?
                    string.Join(",", input.Query.SelectedTypes.Select(n => (int)n)) : null;

                string directions = (input.Query.SelectedDirections != null && input.Query.SelectedDirections.Count > 0) ?
                    string.Join(",", input.Query.SelectedDirections.Select(n => (int)n)) : null;

                ExecuteNonQuerySP("PSTN_BE.sp_SwitchTrunk_CreateTempByFiltered", tempTableName, input.Query.Name, input.Query.Symbol, switchIDs, types, directions, input.Query.IsLinkedToTrunk);

            }, (reader) => TrunkDetailMapper(reader), _mapper);
        }

        public TrunkDetail GetTrunkById(int trunkId)
        {
            return GetItemSP("PSTN_BE.sp_SwitchTrunk_GetByID", TrunkDetailMapper, trunkId);
        }

        public TrunkInfo GetTrunkBySymbol(string symbol)
        {
            return GetItemSP("PSTN_BE.sp_SwitchTrunk_GetBySymbol", TrunkInfoMapper, symbol);
        }

        public List<TrunkInfo> GetTrunksBySwitchIds(TrunkFilter trunkFilterObj)
        {
            string switchIdsString = (trunkFilterObj.SwitchIds != null) ? string.Join<int>(",", trunkFilterObj.SwitchIds) : null;
            return GetItemsSP("PSTN_BE.sp_SwitchTrunk_GetBySwitchIDs", TrunkInfoMapper, switchIdsString, trunkFilterObj.TrunkNameFilter);
        }

        public List<TrunkInfo> GetTrunks()
        {
            return GetItemsSP("PSTN_BE.sp_SwitchTrunk_GetAll", TrunkInfoMapper);
        }

        public List<TrunkInfo> GetTrunksByIds(List<int> trunkIds)
        {
            string commaSeparatedTrunkIds = string.Join<int>(",", trunkIds);
            return GetItemsSP("PSTN_BE.sp_SwitchTrunk_GetByIDs", TrunkInfoMapper, commaSeparatedTrunkIds);
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

        #region Mappers

        TrunkDetail TrunkDetailMapper(IDataReader reader)
        {
            TrunkDetail trunk = new TrunkDetail();

            trunk.TrunkId = (int)reader["ID"];
            trunk.Name = reader["Name"] as string;
            trunk.Symbol = reader["Symbol"] as string;
            trunk.SwitchId = (int)reader["SwitchID"];
            trunk.SwitchName = reader["SwitchName"] as string;
            trunk.Type = (TrunkType)reader["Type"];
            trunk.Direction = (TrunkDirection)reader["Direction"];
            trunk.LinkedToTrunkId = GetReaderValue<int?>(reader, "LinkedToTrunkID");
            trunk.LinkedToTrunkName = GetReaderValue<string>(reader, "LinkedToTrunkName");

            return trunk;
        }

        TrunkInfo TrunkInfoMapper(IDataReader reader)
        {
            TrunkInfo trunk = new TrunkInfo();

            trunk.TrunkId = (int)reader["ID"];
            trunk.Name = reader["Name"] as string;
            trunk.SwitchId = (int)reader["SwitchID"];

            return trunk;
        }

        #endregion
    }
}
