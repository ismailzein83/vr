using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.DBSync.Entities
{
    public enum ToDRateType
    {
        Normal = 0,
        OffPeak = 1,
        Weekend = 2,
        Holiday = 4
    }
    public class SourceTod
    {
        public long TodId { get; set; }
        public string SupplierId { get; set; }
        public string CustomerId { get; set; }
        public int ZoneId { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public ToDRateType RateType { get; set; }
        public DayOfWeek? DayOfWeek { get; set; }
        public DateTime? HolidayDateTime { get; set; }
        public string HolidayName { get; set; }
        public TimeSpan? BeginTime { get; set; }
        public TimeSpan? EndTime { get; set; }
    }
}
