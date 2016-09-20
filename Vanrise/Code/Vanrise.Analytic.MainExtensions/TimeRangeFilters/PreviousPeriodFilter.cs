using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Analytic.Entities;

namespace Vanrise.Analytic.MainExtensions.TimeRangeFilters
{
    public class PreviousPeriodFilter : TimeRangeFilter
    {
        public override Guid ConfigId { get { return  new Guid("BCFFD15E-1A84-46E0-8BD6-45B36049E29E"); } }
        public TimeSpan PeriodLength { get; set; }

        public TimeSpan PeriodDistanceFromNow { get; set; }

        public override void Evaluate(ITimeRangeFilterContext context)
        {
            var toTime = DateTime.Now.Add(-this.PeriodDistanceFromNow);
            context.ToTime = toTime;
            context.FromTime = toTime.Add(-this.PeriodLength);
        }
    }
}
