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
        List<Vanrise.Integration.Entities.DataSource> GetAllDataSources();

        bool AddDataSource(Vanrise.Integration.Entities.DataSource dataSourceObject);

        bool UpdateDataSource(Vanrise.Integration.Entities.DataSource dataSourceObject);

        bool DeleteDataSource(Guid dataSourceId);

        bool UpdateTaskId(Guid dataSourceId, Guid taskId);

        bool UpdateAdapterState(Guid dataSourceId, Entities.BaseAdapterState adapterState);

        bool AreDataSourcesUpdated(ref object updateHandle);
    }
}
