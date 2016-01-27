using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing.MainExtensions.RateType
{
    public class DaysOfWeekRateTypeSettings : PricingRuleRateTypeItemSettings
    {
        public List<DayOfWeek> Days { get; set; }

        public List<Vanrise.Entities.TimeRange> TimeRanges { get; set; }

        protected override bool Evaluate(IPricingRuleRateTypeItemContext context)
        {
            if (context.TargetTime.HasValue && this.Days.Contains(context.TargetTime.Value.DayOfWeek))
            {
                if (this.TimeRanges == null || this.TimeRanges.Count == 0)
                    return true;
                foreach (var timeRange in this.TimeRanges)
                {
                    if (timeRange.IsInRange(context.TargetTime.Value))
                        return true;
                }
            }
            return false;
        }
    }
}
