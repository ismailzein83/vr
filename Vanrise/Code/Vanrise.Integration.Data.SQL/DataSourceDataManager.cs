using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

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
            : base(GetConnectionStringName("IntegrationConnStringKey", "IntegrationDBConnString"))
        {

        }

        public List<Entities.DataSource> GetDataSources()
        {
            return GetItemsSP("integration.sp_DataSource_GetAll", DataSourceMapper);
        }

        public Vanrise.Entities.BigResult<Vanrise.Integration.Entities.DataSource> GetFilteredDataSources(Vanrise.Entities.DataRetrievalInput<object> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("integration.sp_DataSource_CreateTempForFiltered", tempTableName);
            };

            return RetrieveData(input, createTempTableAction, DataSourceMapper, _columnMapper);
        }

        public Entities.DataSource GetDataSource(int dataSourceId)
        {
            return GetItemSP("integration.sp_DataSource_Get", DataSourceMapper, dataSourceId);
        }

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
                Common.Serializer.Serialize(dataSourceObject.AdapterState), Common.Serializer.Serialize(dataSourceObject.Settings));
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



    }
}
