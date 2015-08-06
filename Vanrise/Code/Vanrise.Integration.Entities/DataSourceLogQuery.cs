using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Integration.Entities
{
    public class DataSourceLogQuery
    {
        public int? DataSourceId { get; set; }
        public List<LogEntryTypeEnum> Severities { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
