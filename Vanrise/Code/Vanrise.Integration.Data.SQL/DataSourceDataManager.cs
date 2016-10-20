using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Data.SQL
{
    public class DataSourceDataManager : BaseSQLDataManager, IDataSourceDataManager
    {
        public DataSourceDataManager()
            : base(GetConnectionStringName("IntegrationConfigDBConnStringKey", "IntegrationConfigDBConnString"))
        {

        }
        public List<Vanrise.Integration.Entities.DataSource> GetAllDataSources()
        {
            return GetItemsSP("integration.sp_DataSource_GetAll", DataSourceMapper);
        }
        public bool AreDataSourcesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[integration].[DataSource]", ref updateHandle);
        }

        public bool AddDataSource(Entities.DataSource dataSourceObject)
        {

            int recordesEffected = ExecuteNonQuerySP("integration.sp_DataSource_Insert", dataSourceObject.DataSourceId, dataSourceObject.Name, dataSourceObject.AdapterTypeId,
                Common.Serializer.Serialize(dataSourceObject.AdapterState), dataSourceObject.TaskId, Common.Serializer.Serialize(dataSourceObject.Settings));
                        return (recordesEffected > 0);
        }

        public bool UpdateDataSource(Entities.DataSource dataSourceObject)
        {
            int recordesEffected = ExecuteNonQuerySP("integration.sp_DataSource_Update", dataSourceObject.DataSourceId, dataSourceObject.Name, dataSourceObject.AdapterTypeId,
                Common.Serializer.Serialize(dataSourceObject.Settings));
            return (recordesEffected > 0);
        }

        public bool DeleteDataSource(Guid dataSourceId)
        {
            int recordsEffected = ExecuteNonQuerySP("integration.sp_DataSource_Delete", dataSourceId);
            return (recordsEffected > 0);
        }

        public bool UpdateTaskId(Guid dataSourceId, Guid taskId)
        {
            int recordesEffected = ExecuteNonQuerySP("integration.sp_DataSource_UpdateTaskId", dataSourceId, taskId);
            return (recordesEffected > 0);
        }

        public bool UpdateAdapterState(Guid dataSourceId, Entities.BaseAdapterState adapterState)
        {
            int recordsEffected = ExecuteNonQuerySP("integration.sp_DataSource_UpdateAdapterState", dataSourceId, Common.Serializer.Serialize(adapterState));
            return recordsEffected > 0;
        }


        Vanrise.Integration.Entities.DataSource DataSourceMapper(IDataReader reader)
        {
            Vanrise.Integration.Entities.DataSource dataSource = new Vanrise.Integration.Entities.DataSource
            {
                DataSourceId =GetReaderValue<Guid>(reader,"ID"),
                Name = reader["Name"] as string,
                AdapterTypeId = GetReaderValue<Guid>(reader,"AdapterID"),
                AdapterState = reader["AdapterState"] != DBNull.Value ? Common.Serializer.Deserialize<Vanrise.Integration.Entities.BaseAdapterState>(reader["AdapterState"] as string): null,
                TaskId = GetReaderValue<Guid>(reader,"TaskId"),
                Settings = Common.Serializer.Deserialize<Vanrise.Integration.Entities.DataSourceSettings>(reader["Settings"] as string),
                IsEnabled = GetReaderValue<Boolean>(reader, "IsEnabled"),
            };

            return dataSource;
        }

    }
}
