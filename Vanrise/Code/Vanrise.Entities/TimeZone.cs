using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Entities
{
    public class TimeZone
    {
        public int TimeZoneId { get; set; }
    }

    public class TimeZoneSettings
    {
        public string Title { get; set; }

        public TimeSpan Offset { get; set; }
    }
}
