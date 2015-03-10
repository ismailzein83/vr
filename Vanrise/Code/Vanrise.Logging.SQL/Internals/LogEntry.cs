using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Vanrise.Logging.SQL
{
    internal class LogEntry
    {
        public string AssemblyName { get; set; }

        public string TypeName { get; set; }

        public string MethodName { get; set; }

        public LogEntryType EntryType { get; set; }

        public string Message { get; set; }

        public DateTime EventTime { get; set; }
    }
}
