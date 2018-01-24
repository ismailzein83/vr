using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;

namespace SOM.Main.Entities
{
    public class SOMRequestLog
    {
        public long SOMRequestLogId { get; set; }

        public LogEntryType Severity { get; set; }

        public string Message { get; set; }

        public string ExceptionDetail { get; set; }

        public DateTime EventTime { get; set; }
    }
}
