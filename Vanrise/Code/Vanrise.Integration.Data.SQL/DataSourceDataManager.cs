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
        public DataSourceDataManager()
            : base(GetConnectionStringName("IntegrationConnStringKey", "IntegrationDBConnString"))
        {

        }

        public List<Entities.DataSource> GetDataSources()
        {
            return GetItemsSP("integration.sp_DataSource_GetAll", DataSourceMapper);
        }

        public Entities.DataSource GetDataSource(int dataSourceId)
        {
            return GetItemSP("integration.sp_DataSource_Get", DataSourceMapper, dataSourceId);
        }

        public bool AddDataSource(Entities.DataSource dataSourceObject, out int insertedId)
        {
            object dataSourceId;

            int recordesEffected = ExecuteNonQuerySP("integration.sp_DataSource_Insert", out dataSourceId, dataSourceObject.AdapterTypeId,
                Common.Serializer.Serialize(dataSourceObject.Settings));
            insertedId = (int)dataSourceId;
            return (recordesEffected > 0);
        }

        public bool UpdateDataSource(Entities.DataSource dataSourceObject)
        {
            int recordesEffected = ExecuteNonQuerySP("integration.sp_DataSource_Update", dataSourceObject.DataSourceId, dataSourceObject.AdapterTypeId,
                Common.Serializer.Serialize(dataSourceObject.Settings));
            return (recordesEffected > 0);
        }

        Vanrise.Integration.Entities.DataSource DataSourceMapper(IDataReader reader)
        {
            Vanrise.Integration.Entities.DataSource dataSource = new Vanrise.Integration.Entities.DataSource
            {
                DataSourceId = (int)reader["ID"],
                AdapterTypeId = (int)reader["AdapterID"],
                AdapterName = reader["AdapterName"] as string,
                Settings = Common.Serializer.Deserialize<Vanrise.Integration.Entities.DataSourceSettings>(reader["Settings"] as string),
               
            };
            return dataSource;
        }
    }
}
