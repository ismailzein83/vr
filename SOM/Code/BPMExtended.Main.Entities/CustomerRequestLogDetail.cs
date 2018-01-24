using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace BPMExtended.Main.Entities
{
    public class CustomerRequestLogDetail
    {
        public long RequestLogId { get; set; }

        public LogEntryType Severity { get; set; }

        public string Message { get; set; }

        public string ExceptionDetail { get; set; }

        public DateTime EventTime { get; set; }
    }
}
