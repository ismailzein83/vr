using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TOne.CDRProcess.Arguments
{
    public class TimeRangeRepricingProcessInput
    {
        public TimeRange Range { get; set; }
        public Guid CacheManagerID { get; set; }
    }
}
