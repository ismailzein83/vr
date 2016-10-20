using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Integration.Entities
{
    public interface IDataSourceLogger
    {
        Guid DataSourceId { set; }

        long LogImportedBatchEntry(ImportedBatchEntry enrty);

        void LogEntry(LogEntryType logEntryType, string messageFormat, params object[] args);

        void LogEntry(LogEntryType logEntryType, long importedBatchId, string messageFormat, params object[] args);

        void WriteVerbose(string messageFormat, params object[] args);

        void WriteInformation(string messageFormat, params object[] args);

        void WriteWarning(string messageFormat, params object[] args);

        void WriteError(string messageFormat, params object[] args);
    }
}
