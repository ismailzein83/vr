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
        private static Dictionary<string, string> _columnMapper = new Dictionary<string, string>();

        static DataSourceDataManager()
        {
            _columnMapper.Add("DataSourceId", "ID");
        }

        public DataSourceDataManager()
            : base(GetConnectionStringName("IntegrationConfigDBConnStringKey", "IntegrationConfigDBConnString"))
        {

        }

        public List<Vanrise.Integration.Entities.DataSource> GetAllDataSources()
        {
            return GetItemsSP("integration.sp_DataSource_GetAll", DataSourceMapper);
        }

        public List<Entities.DataSourceInfo> GetDataSources()
        {
            return GetItemsSP("integration.sp_DataSource_GetAll", DataSourceInfoMapper);
        }

        public Vanrise.Entities.BigResult<Vanrise.Integration.Entities.DataSourceDetail> GetFilteredDataSources(Vanrise.Entities.DataRetrievalInput<DataSourceQuery> input)
        {
            return RetrieveData(input, (tempTableName) =>
            {
                string adapterTypeIDs = (input.Query.AdapterTypeIDs != null && input.Query.AdapterTypeIDs.Count() > 0) ?
                    string.Join<int>(",", input.Query.AdapterTypeIDs) : null;

                ExecuteNonQuerySP("integration.sp_DataSource_CreateTemp", tempTableName, input.Query.Name, adapterTypeIDs, input.Query.IsEnabled);

            }, (reader) => DataSourceDetailMapper(reader), _columnMapper);
        }

        public Entities.DataSourceDetail GetDataSource(int dataSourceId)
        {
            return GetItemSP("integration.sp_DataSource_Get", DataSourceDetailMapper, dataSourceId);
        }


        public bool AreDataSourcesUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[integration].[DataSource]", ref updateHandle);
        }

        //public List<Entities.DataSource> GetDataSources()
        //{
        //    return GetItemsSP("integration.sp_DataSource_GetAll", DataSourceMapper);
        //}

        //public Entities.DataSource GetDataSource(int dataSourceId)
        //{
        //    return GetItemSP("integration.sp_DataSource_Get", DataSourceMapper, dataSourceId);
        //}
        
        public Entities.DataSource GetDataSourcebyTaskId(int taskId)
        {
            return GetItemSP("integration.sp_DataSource_GetByTaskId", DataSourceMapper, taskId);
        }

        public bool AddDataSource(Entities.DataSource dataSourceObject, out int insertedId)
        {
            object dataSourceId;

            int recordesEffected = ExecuteNonQuerySP("integration.sp_DataSource_Insert", out dataSourceId, dataSourceObject.Name, dataSourceObject.AdapterTypeId,
                Common.Serializer.Serialize(dataSourceObject.AdapterState), dataSourceObject.TaskId, Common.Serializer.Serialize(dataSourceObject.Settings));
            
            insertedId = (int)dataSourceId;
            return (recordesEffected > 0);
        }

        public bool UpdateDataSource(Entities.DataSource dataSourceObject)
        {
            int recordesEffected = ExecuteNonQuerySP("integration.sp_DataSource_Update", dataSourceObject.DataSourceId, dataSourceObject.Name, dataSourceObject.AdapterTypeId,
                Common.Serializer.Serialize(dataSourceObject.Settings));
            return (recordesEffected > 0);
        }

        public bool DeleteDataSource(int dataSourceId)
        {
            int recordsEffected = ExecuteNonQuerySP("integration.sp_DataSource_Delete", dataSourceId);
            return (recordsEffected > 0);
        }

        public bool UpdateTaskId(int dataSourceId, int taskId)
        {
            int recordesEffected = ExecuteNonQuerySP("integration.sp_DataSource_UpdateTaskId", dataSourceId, taskId);
            return (recordesEffected > 0);
        }

        public bool UpdateAdapterState(int dataSourceId, Entities.BaseAdapterState adapterState)
        {
            int recordsEffected = ExecuteNonQuerySP("integration.sp_DataSource_UpdateAdapterState", dataSourceId, Common.Serializer.Serialize(adapterState));
            return recordsEffected > 0;
        }

        Vanrise.Integration.Entities.DataSourceDetail DataSourceDetailMapper(IDataReader reader)
        {
            Vanrise.Integration.Entities.DataSourceDetail dataSourceDetail = new Vanrise.Integration.Entities.DataSourceDetail
            {
                DataSourceId = (int)reader["ID"],
                Name = reader["Name"] as string,
                AdapterTypeId = (int)reader["AdapterID"],
                AdapterName = reader["AdapterName"] as string,
                AdapterInfo = Common.Serializer.Deserialize<Vanrise.Integration.Entities.AdapterTypeInfo>(reader["Info"] as string),
                AdapterState = reader["AdapterState"] != DBNull.Value ? Common.Serializer.Deserialize<Vanrise.Integration.Entities.BaseAdapterState>(reader["AdapterState"] as string) : null,
                TaskId = (int)reader["TaskId"],
                IsEnabled = (bool)reader["IsEnabled"],
                Settings = Common.Serializer.Deserialize<Vanrise.Integration.Entities.DataSourceSettings>(reader["Settings"] as string)
            };

            return dataSourceDetail;
        }

        Vanrise.Integration.Entities.DataSource DataSourceMapper(IDataReader reader)
        {
            Vanrise.Integration.Entities.DataSource dataSource = new Vanrise.Integration.Entities.DataSource
            {
                DataSourceId = (int)reader["ID"],
                Name = reader["Name"] as string,
                AdapterTypeId = (int)reader["AdapterID"],
                AdapterInfo = Common.Serializer.Deserialize<Vanrise.Integration.Entities.AdapterTypeInfo>(reader["Info"] as string),
                AdapterState = reader["AdapterState"] != DBNull.Value ? Common.Serializer.Deserialize<Vanrise.Integration.Entities.BaseAdapterState>(reader["AdapterState"] as string): null,
                TaskId = (int)reader["TaskId"],
                Settings = Common.Serializer.Deserialize<Vanrise.Integration.Entities.DataSourceSettings>(reader["Settings"] as string)
            };

            return dataSource;
        }

        DataSourceInfo DataSourceInfoMapper(IDataReader reader)
        {
            DataSourceInfo dataSourceInfo = new DataSourceInfo
            {
                DataSourceID = (int)reader["ID"],
                Name = reader["Name"] as string
            };

            return dataSourceInfo;
        }
    }
}
