using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.DBSync.Entities
{
    public class SourceTod
    {
        public int TodId { get; set; }
        public string SupplierId { get; set; }
        public string CustomerId { get; set; }
        public int ZoneId { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public short RateType { get; set; }
        public short WeekDay { get; set; }
        public DateTime? HolidayDateTime { get; set; }
        public string HolidayName { get; set; }
        public string BeginTime { get; set; }
        public string EndTime { get; set; }
    }
}
