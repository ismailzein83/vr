using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Data.SQL;

namespace Vanrise.Integration.Data.SQL
{
    public class DataSourceAdapterTypeDataManager : BaseSQLDataManager, IDataSourceAdapterTypeDataManager
    {
        public DataSourceAdapterTypeDataManager()
            : base(GetConnectionStringName("IntegrationConfigDBConnStringKey", "IntegrationConfigDBConnString"))
        {

        }

        public List<Vanrise.Integration.Entities.DataSourceAdapterType> GetDataSourceAdapterTypes()
        {
            return GetItemsSP("integration.sp_AdapterType_GetAll", DataSourceAdapterTypeMapper);
        }

        Vanrise.Integration.Entities.DataSourceAdapterType DataSourceAdapterTypeMapper(IDataReader reader)
        {
            Vanrise.Integration.Entities.DataSourceAdapterType adapterType = new Vanrise.Integration.Entities.DataSourceAdapterType
            {
                AdapterTypeId = (int)reader["ID"],
                Name = reader["Name"] as string,
                Info = Serializer.Deserialize<Vanrise.Integration.Entities.AdapterTypeInfo>(reader["Info"] as string)
            };
            return adapterType;
        }
    }
}
