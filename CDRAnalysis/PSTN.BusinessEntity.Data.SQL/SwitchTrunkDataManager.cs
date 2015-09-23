using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PSTN.BusinessEntity.Data.SQL
{
    public class SwitchTrunkDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISwitchTrunkDataManager
    {
        public SwitchTrunkDataManager() : base("CDRDBConnectionString") { }

        public Vanrise.Entities.BigResult<SwitchTrunkDetail> GetFilteredSwitchTrunks(Vanrise.Entities.DataRetrievalInput<SwitchTrunkDetailQuery> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("TypeDescription", "Type");
            mapper.Add("DirectionDescription", "Direction");

            return RetrieveData(input, (tempTableName) =>
            {
                string switchIDs = (input.Query.SelectedSwitchIDs != null && input.Query.SelectedSwitchIDs.Count() > 0) ?
                    string.Join<int>(",", input.Query.SelectedSwitchIDs) : null;

                string types = (input.Query.SelectedTypes != null && input.Query.SelectedTypes.Count > 0) ?
                    string.Join(",", input.Query.SelectedTypes.Select(n => (int)n)) : null;

                string directions = (input.Query.SelectedDirections != null && input.Query.SelectedDirections.Count > 0) ?
                    string.Join(",", input.Query.SelectedDirections.Select(n => (int)n)) : null;

                ExecuteNonQuerySP("PSTN_BE.sp_SwitchTrunk_CreateTempByFiltered", tempTableName, input.Query.Name, input.Query.Symbol, switchIDs, types, directions, input.Query.IsLinkedToTrunk);

            }, (reader) => SwitchTrunkDetailMapper(reader), mapper);
        }

        public SwitchTrunkDetail GetSwitchTrunkByID(int trunkID)
        {
            return GetItemSP("PSTN_BE.sp_SwitchTrunk_GetByID", SwitchTrunkDetailMapper, trunkID);
        }

        public List<SwitchTrunkInfo> GetSwitchTrunksBySwitchID(int switchID)
        {
            return GetItemsSP("PSTN_BE.sp_SwitchTrunk_GetBySwitchID", SwitchTrunkInfoMapper, switchID);
        }

        public bool AddSwitchTrunk(SwitchTrunk trunkObject, out int insertedID)
        {
            object trunkID;

            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_SwitchTrunk_Insert", out trunkID, trunkObject.Name, trunkObject.Symbol, trunkObject.SwitchID, trunkObject.Type, trunkObject.Direction);

            insertedID = (recordsAffected > 0) ? (int)trunkID : -1;
            return (recordsAffected > 0);
        }

        public bool UpdateSwitchTrunk(SwitchTrunk trunkObject)
        {
            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_SwitchTrunk_Update", trunkObject.ID, trunkObject.Name, trunkObject.Symbol, trunkObject.SwitchID, trunkObject.Type, trunkObject.Direction);
            return (recordsAffected > 0);
        }

        public bool DeleteSwitchTrunk(int trunkID)
        {
            int recordsEffected = ExecuteNonQuerySP("PSTN_BE.sp_SwitchTrunk_Delete", trunkID);
            return (recordsEffected > 0);
        }

        public void UnlinkSwitchTrunk(int switchTrunkID, int linkedToTrunkID)
        {
            ExecuteNonQuerySP("PSTN_BE.sp_SwitchTrunk_Unlink", switchTrunkID, linkedToTrunkID);
        }

        public void LinkSwitchTrunks(int switchTrunkID, int linkedToTrunkID)
        {
            ExecuteNonQuerySP("PSTN_BE.sp_SwitchTrunk_LinkToTrunk", switchTrunkID, linkedToTrunkID);
        }

        #region Mappers

        SwitchTrunkDetail SwitchTrunkDetailMapper(IDataReader reader)
        {
            SwitchTrunkDetail trunk = new SwitchTrunkDetail();

            trunk.ID = (int)reader["ID"];
            trunk.Name = reader["Name"] as string;
            trunk.Symbol = reader["Symbol"] as string;
            trunk.SwitchID = (int)reader["SwitchID"];
            trunk.SwitchName = reader["SwitchName"] as string;
            trunk.Type = (SwitchTrunkType)reader["Type"];
            trunk.Direction = (SwitchTrunkDirection)reader["Direction"];
            trunk.LinkedToTrunkID = GetReaderValue<int?>(reader, "LinkedToTrunkID");
            trunk.LinkedToTrunkName = GetReaderValue<string>(reader, "LinkedToTrunkName");

            return trunk;
        }

        SwitchTrunkInfo SwitchTrunkInfoMapper(IDataReader reader)
        {
            SwitchTrunkInfo trunk = new SwitchTrunkInfo();

            trunk.ID = (int)reader["ID"];
            trunk.Name = reader["Name"] as string;

            return trunk;
        }

        #endregion
    }
}
