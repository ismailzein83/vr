﻿using PSTN.BusinessEntity.Entities;
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
                string typeIDs = (input.Query.SelectedTypeIds != null && input.Query.SelectedTypeIds.Count() > 0) ?
                    string.Join<int>(",", input.Query.SelectedTypeIds) : null;

                ExecuteNonQuerySP("PSTN_BE.sp_Switch_CreateTempByFiltered", tempTableName, input.Query.Name, typeIDs, input.Query.AreaCode);

            }, (reader) => SwitchDetailMapper(reader), _mapper);
        }
        
        public SwitchDetail GetSwitchById(int switchId) {
            return GetItemSP("PSTN_BE.sp_Switch_GetByID", SwitchDetailMapper, switchId);
        }

        public Switch GetSwitchByDataSourceId(int DataSourceId)
        {
            return GetItemSP("PSTN_BE.sp_Switch_GetByDataSourceID", SwitchMapper, DataSourceId);
        }

        public List<SwitchInfo> GetSwitchesByIds(List<int> switchIds)
        {
            string commaSeparatedSwitchIds = string.Join<int>(",", switchIds);
            return GetItemsSP("PSTN_BE.sp_Switch_GetByIDs", SwitchInfoMapper, commaSeparatedSwitchIds);
        }

        public List<SwitchInfo> GetSwitches()
        {
            return GetItemsSP("PSTN_BE.sp_Switch_GetAll", SwitchInfoMapper);
        }

        public List<SwitchAssignedDataSource> GetSwitchAssignedDataSources()
        {
            return GetItemsSP("PSTN_BE.sp_Switch_GetSwitchAssignedDataSources", SwitchAssignedDataSourceMapper);
        }

        public List<SwitchInfo> GetSwitchesToLinkTo(int switchId)
        {
            return GetItemsSP("PSTN_BE.sp_Switch_GetToLinkTo", SwitchInfoMapper, switchId);
        }

        public bool UpdateSwitch(Switch switchObj)
        {
            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_Switch_Update", switchObj.SwitchId, switchObj.Name, switchObj.TypeId, switchObj.AreaCode, switchObj.TimeOffset.ToString(), switchObj.DataSourceId);

            return (recordsAffected > 0);
        }

        public bool AddSwitch(Switch switchObj, out int insertedId)
        {
            object switchId;

            int recordsAffected = ExecuteNonQuerySP("PSTN_BE.sp_Switch_Insert", out switchId, switchObj.Name, switchObj.TypeId, switchObj.AreaCode, switchObj.TimeOffset.ToString(), switchObj.DataSourceId);

            insertedId = (recordsAffected > 0) ? (int)switchId : -1;
            return (recordsAffected > 0);
        }

        public bool DeleteSwitch(int switchId)
        {
            int recordsEffected = ExecuteNonQuerySP("PSTN_BE.sp_Switch_Delete", switchId);
            return (recordsEffected > 0);
        }

        #region Mappers

        private Switch SwitchMapper(IDataReader reader)
        {
            Switch switchObject = new Switch();

            switchObject.SwitchId = (int)reader["ID"];
            switchObject.Name = reader["Name"] as string;
            switchObject.TypeId = (int)reader["TypeID"];
            switchObject.AreaCode = reader["AreaCode"] as string;
            switchObject.TimeOffset = TimeSpan.Parse(reader["TimeOffset"] as string);
            switchObject.DataSourceId = GetReaderValue<int?>(reader, "DataSourceID");

            return switchObject;
        }

        private SwitchAssignedDataSource SwitchAssignedDataSourceMapper(IDataReader reader)
        {
            SwitchAssignedDataSource dataSourceObject = new SwitchAssignedDataSource();

            dataSourceObject.DataSourceId = (int)reader["DataSourceID"];

            return dataSourceObject;
        }

        private SwitchDetail SwitchDetailMapper(IDataReader reader)
        {
            SwitchDetail switchObject = new SwitchDetail();

            switchObject.SwitchId = (int)reader["ID"];
            switchObject.Name = reader["Name"] as string;
            switchObject.TypeId = (int)reader["TypeID"];
            switchObject.TypeName = reader["TypeName"] as string;
            switchObject.AreaCode = reader["AreaCode"] as string;
            switchObject.TimeOffset = TimeSpan.Parse(reader["TimeOffset"] as string);
            switchObject.DataSourceId = GetReaderValue<int?>(reader, "DataSourceID");

            return switchObject;
        }

        private SwitchInfo SwitchInfoMapper(IDataReader reader)
        {
            SwitchInfo switchObject = new SwitchInfo();

            switchObject.SwitchId = (int)reader["ID"];
            switchObject.Name = reader["Name"] as string;

            return switchObject;
        }

        #endregion
    }
}
