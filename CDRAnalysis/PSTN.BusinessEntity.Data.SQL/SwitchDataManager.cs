using PSTN.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;

namespace PSTN.BusinessEntity.Data.SQL
{
    public class SwitchDataManager : Vanrise.Data.SQL.BaseSQLDataManager, ISwitchDataManager
    {
        private Dictionary<string, string> _mapper;

        public SwitchDataManager()
            : base("CDRDBConnectionString")
        {
            _mapper = new Dictionary<string, string>();
            _mapper.Add("DataSourceName", "DataSourceID");
            _mapper.Add("BrandName", "TypeName");
        }


        public List<Switch> GetSwitches()
        {
            return GetItemsSP("PSTN_BE.sp_Switch_GetAll", SwitchMapper);
        }

        public bool AddSwitch(Switch switchObj, out int insertedId)
        {
            object switchId;

            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_Switch_Insert", out switchId, switchObj.Name, switchObj.BrandId, switchObj.AreaCode, switchObj.TimeOffset.ToString(), switchObj.DataSourceId);

            insertedId = (recordsAffected > 0) ? (int)switchId : -1;
            return (recordsAffected > 0);
        }

        public bool UpdateSwitch(Switch switchObj)
        {
            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_Switch_Update", switchObj.SwitchId, switchObj.Name, switchObj.BrandId, switchObj.AreaCode, switchObj.TimeOffset.ToString(), switchObj.DataSourceId);

            return (recordsAffected > 0);
        }

        public bool DeleteSwitch(int switchId)
        {
            int recordsEffected = ExecuteNonQuerySP("PSTN_BE.sp_Switch_Delete", switchId);
            return (recordsEffected > 0);
        }

        public bool AreSwitchesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("PSTN_BE.Switch", ref updateHandle);
        }


        #region Mappers

        private Switch SwitchMapper(IDataReader reader)
        {
            Switch switchObject = new Switch();

            switchObject.SwitchId = (int)reader["ID"];
            switchObject.Name = reader["Name"] as string;
            switchObject.BrandId = (int)reader["TypeID"];
            switchObject.AreaCode = reader["AreaCode"] as string;
            switchObject.TimeOffset = TimeSpan.Parse(reader["TimeOffset"] as string);
            switchObject.DataSourceId = GetReaderValue<Guid?>(reader, "DataSourceID");

            return switchObject;
        }

        #endregion
    }
}
