using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Integration.Entities
{
    public class DataSourceLogQuery
    {
        public Guid? DataSourceId { get; set; }
        public List<LogEntryType> Severities { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int Top { get; set; }
    }
}
