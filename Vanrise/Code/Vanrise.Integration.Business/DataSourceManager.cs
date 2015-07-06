using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Data;

namespace Vanrise.Integration.Business
{
    public class DataSourceManager
    {
        public List<Vanrise.Integration.Entities.DataSource> GetDataSources()
        {
            IDataSourceDataManager datamanager = IntegrationDataManagerFactory.GetDataManager<IDataSourceDataManager>();
            return datamanager.GetDataSources();
        }
    }
}
