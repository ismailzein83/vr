using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Data
{
    public interface IDataSourceLogDataManager : IDataManager
    {
        void WriteEntry(Vanrise.Common.LogEntryType entryType, string message, int dataSourceId, int? queueItemId, DateTime logTimeSpan);
    }
}
