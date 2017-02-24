using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Common
{
    public abstract class LogHandler
    {
        internal LogEntryType? LogLevel { get; set; }

        internal protected abstract void WriteEntry(string eventType, string exceptionDetail, LogEntryType entryType, string message, string callingModule, string callingType, string callingMethod);
    }
}
