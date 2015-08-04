using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Integration.Entities
{
    public interface IDataSourceLogger
    {
        int DataSourceId { set; }

        void LogEntry(LogEntryType logEntryType, string messageFormat, params object[] args);

        void LogEntry(LogEntryType logEntryType, int queteItemId, string messageFormat, params object[] args);

        void WriteVerbose(string messageFormat, params object[] args);

        void WriteInformation(string messageFormat, params object[] args);

        void WriteWarning(string messageFormat, params object[] args);

        void WriteError(string messageFormat, params object[] args);
    }
}
