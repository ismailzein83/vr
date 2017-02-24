using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace Vanrise.Entities
{
    public class LogEntry
    {
        public long LogEntryId { get; set; }

        public int MachineId { get; set; }

        public int ApplicationId { get; set; }

        public int TypeId { get; set; }

        public int MethodId { get; set; }

        public int AssemblyId { get; set; }

        public LogEntryType EntryType { get; set; }

        public string Message { get; set; }

        public string ExceptionDetail { get; set; }

        public DateTime EventTime { get; set; }

        public int? EventType { get; set; }

    }
}
