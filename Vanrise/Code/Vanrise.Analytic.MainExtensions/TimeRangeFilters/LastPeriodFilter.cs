using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.TimeRangeFilters
{
    public class LastPeriodFilter : TimeRangeFilter
    {
        public TimeSpan PeriodLength { get; set; }
        
        public override void Evaluate(ITimeRangeFilterContext context)
        {
            var toTime = DateTime.Now;
            context.ToTime = toTime;
            context.FromTime = toTime.Add(-this.PeriodLength);
        }
    }
}
