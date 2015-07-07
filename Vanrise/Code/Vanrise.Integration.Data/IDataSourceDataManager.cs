using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Data
{
    public interface IDataSourceDataManager : IDataManager
    {
        List<Vanrise.Integration.Entities.DataSource> GetDataSources();

        Vanrise.Integration.Entities.DataSource GetDataSource(int dataSourceId);

        bool AddDataSource(Vanrise.Integration.Entities.DataSource dataSourceObject, out int insertedId);

        bool UpdateDataSource(Vanrise.Integration.Entities.DataSource dataSourceObject);

        bool UpdateTaskId(int dataSourceId, int taskId);
    }
}
