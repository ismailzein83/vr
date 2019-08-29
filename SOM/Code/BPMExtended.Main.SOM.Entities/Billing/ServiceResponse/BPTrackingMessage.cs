using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BPMExtended.Main.SOMAPI
{
    public class BPTrackingMessage
    {
        public long Id { get; set; }
        public long ProcessInstanceId { get; set; }
        public long? ParentProcessId { get; set; }
        public LogEntryType Severity { get; set; }
        public string TrackingMessage { get; set; }
        public string ExceptionDetail { get; set; }
        public DateTime EventTime { get; set; }
    }

    public enum LogEntryType
    {
        Error = 1,
        Warning = 2,
        Information = 4,
        Verbose = 8
    }
}
