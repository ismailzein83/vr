using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Data
{
    public interface IDataSourceStateDataManager : IDataManager
    {
        string GetDataSourceState(Guid dataSourceId);

        void InsertOrUpdateDataSourceState(Guid dataSourceId, string dataSourceState);
    }
}
