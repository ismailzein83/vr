using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Common
{
    public abstract class LogHandler
    {
        internal LogEntryType? LogLevel { get; set; }

        internal protected abstract void WriteEntry(LogEntryType entryType, string message, string callingModule, string callingType, string callingMethod);
    }
}
