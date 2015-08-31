using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public interface IDataSourceManager
    {
        bool UpdateAdapterState(int dataSourceId, Vanrise.Integration.Entities.BaseAdapterState adapterState);
    }
}
