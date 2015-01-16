using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TOne.RepricingProcess.Activities;

namespace TOne.RepricingProcess
{
    public class TimeRangeRepricingProcessInput
    {
        public TimeRange Range { get; set; }
        public Guid CacheManagerID { get; set; }
    }
}
