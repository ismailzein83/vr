using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class TimeRange
    {
        public Time FromTime { get; set; }

        public Time ToTime { get; set; }

        public bool IsInRange(DateTime time)
        {
            TimeSpan from = new TimeSpan(0, this.FromTime.Hour, this.FromTime.Minute, this.FromTime.Second, this.FromTime.MilliSecond);
            TimeSpan to = new TimeSpan(0, this.ToTime.Hour, this.ToTime.Minute, this.ToTime.Second, this.ToTime.MilliSecond);
            TimeSpan timeSpan = time.TimeOfDay;

            return timeSpan >= from && timeSpan <= to;
        }
    }
}
