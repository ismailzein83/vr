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
            //Thread.Sleep(3000);
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
        
        public Switch GetSwitchByID(int switchID) {
            return GetItemSP("PSTN_BE.sp_Switch_GetByID", SwitchMapper, switchID);
        }

        public List<Switch> GetSwitches()
        {
            return GetItemsSP("PSTN_BE.sp_Switch_GetAll", SwitchMapper);
        }

        public bool UpdateSwitch(Switch switchObject)
        {
            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_Switch_Update", switchObject.ID, switchObject.Name, switchObject.TypeID, switchObject.AreaCode, switchObject.TimeOffset.ToString());
            return (recordsAffected > 0);
        }

        public bool AddSwitch(Switch switchObject, out int insertedID)
        {
            object switchID;

            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_Switch_Insert", out switchID, switchObject.Name, switchObject.TypeID, switchObject.AreaCode, switchObject.TimeOffset.ToString());

            insertedID = (recordsAffected > 0) ? (int)switchID : -1;
            return (recordsAffected > 0);
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
            switchObject.TypeID = (int)reader["TypeID"];
            switchObject.AreaCode = reader["AreaCode"] as string;
            switchObject.TimeOffset = reader["TimeOffset"] as string;

            return switchObject;
        }

        #endregion
    }
}
