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

                ExecuteNonQuerySP("PSTN_BE.sp_SwitchTrunk_CreateTempByFiltered", tempTableName, input.Query.Name, input.Query.Symbol, switchIDs, types, directions);

            }, (reader) => SwitchTrunkDetailMapper(reader), mapper);
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

            return trunk;
        }

        #endregion
    }
}
