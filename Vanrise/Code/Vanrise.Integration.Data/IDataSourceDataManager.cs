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
    }
}
