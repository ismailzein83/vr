using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.PricingRules.RateTypeSettings
{
    public class DaysOfWeekRateTypeSettings : PricingRuleRateTypeItemSettings
    {
        public List<DayOfWeek> Days { get; set; }

        public List<Vanrise.Entities.TimeRange> TimeRanges { get; set; }

        public override bool Evaluate(IPricingRuleRateTypeItemContext context, PricingRuleRateTypeTarget target)
        {
            if (target.EffectiveOn.HasValue && this.Days.Contains(target.EffectiveOn.Value.DayOfWeek))
            {
                if (this.TimeRanges == null || this.TimeRanges.Count == 0)
                    return true;
                foreach(var timeRange in this.TimeRanges)
                {
                    if (timeRange.IsInRange(target.EffectiveOn.Value))
                        return true;
                }
            }
            return false;
        }
    }
}
