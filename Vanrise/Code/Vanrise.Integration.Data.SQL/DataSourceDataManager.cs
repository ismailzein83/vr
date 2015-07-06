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

        Vanrise.Integration.Entities.DataSource DataSourceMapper(IDataReader reader)
        {
            Vanrise.Integration.Entities.DataSource dataSource = new Vanrise.Integration.Entities.DataSource
            {
                DataSourceId = (int)reader["ID"],
                AdapterName = reader["AdapterName"] as string,
               
            };
            return dataSource;
        }
    }
}
