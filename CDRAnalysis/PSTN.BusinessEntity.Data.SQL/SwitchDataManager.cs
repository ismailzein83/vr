using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace PSTN.BusinessEntity.Data.SQL
{
    public class SwitchDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISwitchDataManager
    {
        private Dictionary<string, string> _mapper;

        public SwitchDataManager() : base("CDRDBConnectionString") {
            _mapper = new Dictionary<string, string>();

            _mapper.Add("TypeDescription", "TypeID");
            _mapper.Add("DataSourceName", "DataSourceID");
        }

        public Vanrise.Entities.BigResult<SwitchDetail> GetFilteredSwitches(Vanrise.Entities.DataRetrievalInput<SwitchQuery> input)
        {
            return RetrieveData(input, (tempTableName) =>
            {
                string typeIDs = (input.Query.SelectedTypeIDs != null && input.Query.SelectedTypeIDs.Count() > 0) ?
                    string.Join<int>(",", input.Query.SelectedTypeIDs) : null;

                ExecuteNonQuerySP("PSTN_BE.sp_Switch_CreateTempByFiltered", tempTableName, input.Query.Name, typeIDs, input.Query.AreaCode);

            }, (reader) => SwitchDetailMapper(reader), _mapper);
        }
        
        public SwitchDetail GetSwitchByID(int switchID) {
            return GetItemSP("PSTN_BE.sp_Switch_GetByID", SwitchDetailMapper, switchID);
        }

        public Switch GetSwitchByDataSourceID(int DataSourceID)
        {
            return GetItemSP("PSTN_BE.sp_Switch_GetByDataSourceID", SwitchMapper, DataSourceID);
        }

        public List<SwitchInfo> GetSwitches()
        {
            return GetItemsSP("PSTN_BE.sp_Switch_GetAll", SwitchInfoMapper);
        }

        public List<SwitchInfo> GetSwitchesToLinkTo(int switchID)
        {
            return GetItemsSP("PSTN_BE.sp_Switch_GetToLinkTo", SwitchInfoMapper, switchID);
        }

        public bool UpdateSwitch(Switch switchObject)
        {
            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_Switch_Update", switchObject.ID, switchObject.Name, switchObject.TypeID, switchObject.AreaCode, switchObject.TimeOffset.ToString(), switchObject.DataSourceID);
            return (recordsAffected > 0);
        }

        public bool AddSwitch(Switch switchObject, out int insertedID)
        {
            object switchID;

            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_Switch_Insert", out switchID, switchObject.Name, switchObject.TypeID, switchObject.AreaCode, switchObject.TimeOffset.ToString(), switchObject.DataSourceID);

            insertedID = (recordsAffected > 0) ? (int)switchID : -1;
            return (recordsAffected > 0);
        }

        public bool DeleteSwitch(int switchID)
        {
            int recordsEffected = ExecuteNonQuerySP("PSTN_BE.sp_Switch_Delete", switchID);
            return (recordsEffected > 0);
        }

        #region Mappers

        Switch SwitchMapper(IDataReader reader)
        {
            Switch switchObject = new Switch();

            switchObject.ID = (int)reader["ID"];
            switchObject.Name = reader["Name"] as string;
            switchObject.TypeID = (int)reader["TypeID"];
            switchObject.AreaCode = reader["AreaCode"] as string;
            switchObject.TimeOffset = TimeSpan.Parse(reader["TimeOffset"] as string);
            switchObject.DataSourceID = GetReaderValue<int?>(reader, "DataSourceID");

            return switchObject;
        }


        SwitchDetail SwitchDetailMapper(IDataReader reader)
        {
            SwitchDetail switchObject = new SwitchDetail();

            switchObject.ID = (int)reader["ID"];
            switchObject.Name = reader["Name"] as string;
            switchObject.TypeID = (int)reader["TypeID"];
            switchObject.TypeName = reader["TypeName"] as string;
            switchObject.AreaCode = reader["AreaCode"] as string;
            switchObject.TimeOffset = TimeSpan.Parse(reader["TimeOffset"] as string);
            switchObject.DataSourceID = GetReaderValue<int?>(reader, "DataSourceID");

            return switchObject;
        }

        SwitchInfo SwitchInfoMapper(IDataReader reader)
        {
            SwitchInfo switchObject = new SwitchInfo();

            switchObject.ID = (int)reader["ID"];
            switchObject.Name = reader["Name"] as string;

            return switchObject;
        }

        #endregion
    }
}
