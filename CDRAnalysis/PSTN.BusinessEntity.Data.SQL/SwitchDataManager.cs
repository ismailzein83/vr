using PSTN.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PSTN.BusinessEntity.Data.SQL
{
    public class SwitchDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISwitchDataManager
    {
        public SwitchDataManager() : base("CDRDBConnectionString") { }

        public List<SwitchType> GetSwitchTypes()
        {
            return GetItemsSP("PSTN_BE.sp_SwitchType_GetAll", SwitchTypeMapper);
        }

        public Vanrise.Entities.BigResult<Switch> GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input)
        {
            Dictionary<string, string> mapper = new Dictionary<string, string>();
            mapper.Add("TypeDescription", "TypeID");

            return RetrieveData(input, (tempTableName) =>
            {
                string typeIDs = (input.Query.SelectedTypeIDs != null && input.Query.SelectedTypeIDs.Count() > 0) ?
                    string.Join<int>(",", input.Query.SelectedTypeIDs) : null;

                ExecuteNonQuerySP("PSTN_BE.sp_Switch_CreateTempByFiltered", tempTableName, input.Query.Name, typeIDs, input.Query.AreaCode);

            }, (reader) => SwitchMapper(reader), mapper);
        }

        #region Mappers

        SwitchType SwitchTypeMapper(IDataReader reader)
        {
            SwitchType type = new SwitchType();
            
            type.ID = (int)reader["ID"];
            type.Name = reader["Name"] as string;

            return type;
        }

        Switch SwitchMapper(IDataReader reader)
        {
            Switch switchObject = new Switch();

            switchObject.ID = (int)reader["ID"];
            switchObject.Name = reader["Name"] as string;
            switchObject.TypeID = GetReaderValue<int?>(reader, "TypeID");
            switchObject.AreaCode = GetReaderValue<string>(reader, "AreaCode");

            return switchObject;
        }

        #endregion
    }
}
