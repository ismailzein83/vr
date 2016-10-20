using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Integration.Entities;

namespace Vanrise.Integration.Data
{
    public interface IDataSourceLogDataManager : IDataManager
    {
        void InsertEntry(Vanrise.Entities.LogEntryType entryType, string message, Guid dataSourceId, long? queueItemId, string logTimeSpan);

        Vanrise.Entities.BigResult<DataSourceLog> GetFilteredDataSourceLogs(Vanrise.Entities.DataRetrievalInput<DataSourceLogQuery> input);
    }
}
