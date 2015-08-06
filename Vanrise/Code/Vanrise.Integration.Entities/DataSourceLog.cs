using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class DataSourceLog
    {
        public int ID { get; set; }
        public int DataSourceId { get; set; }
        public string DataSourceName { get; set; }
        public LogEntryTypeEnum Severity { get; set; }
        public string Message { get; set; }
        public DateTime LogEntryTime { get; set; }
    }
}
