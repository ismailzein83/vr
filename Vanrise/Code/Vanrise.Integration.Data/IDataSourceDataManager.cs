using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Data
{
    public interface IDataSourceDataManager : IDataManager
    {
        List<Vanrise.Integration.Entities.DataSourceInfo> GetDataSources();

        Vanrise.Entities.BigResult<Vanrise.Integration.Entities.DataSourceDetail> GetFilteredDataSources(Vanrise.Entities.DataRetrievalInput<DataSourceQuery> input);

        Vanrise.Integration.Entities.DataSourceDetail GetDataSource(int dataSourceId);

        Vanrise.Integration.Entities.DataSource GetDataSourcebyTaskId(int taskId);

        bool AddDataSource(Vanrise.Integration.Entities.DataSource dataSourceObject, out int insertedId);

        bool UpdateDataSource(Vanrise.Integration.Entities.DataSource dataSourceObject);

        bool DeleteDataSource(int dataSourceId);

        bool UpdateTaskId(int dataSourceId, int taskId);

        bool UpdateAdapterState(int dataSourceId, Entities.BaseAdapterState adapterState);
    }
}
