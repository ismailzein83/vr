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
        void LogEntry(LogEntryType logEntryType, string message);
    }
}
