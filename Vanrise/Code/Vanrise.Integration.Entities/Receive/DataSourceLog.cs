using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Integration.Entities
{
    public class DataSourceLog
    {
        public int ID { get; set; }
        public Guid DataSourceId { get; set; }
        public string DataSourceName { get; set; }
        public LogEntryType Severity { get; set; }
        public string Message { get; set; }
        public DateTime LogEntryTime { get; set; }
    }
}
