using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing.MainExtensions.RateType
{
    public class DaysOfWeekRateTypeSettings : PricingRuleRateTypeItemSettings
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("d642aa26-c072-43ef-98f1-ee84f05f4069"); } }
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

        public override string GetDescription()
        {
            StringBuilder description = new StringBuilder();

            if (Days != null && Days.Count > 0)
            {
                description.Append("Days: ");
                IEnumerable<string> dayDescriptions = Days.Select(day => day.ToString());
                description.Append(String.Join(", ", dayDescriptions));
            }

            if (TimeRanges != null && TimeRanges.Count > 0)
            {
                description.Append("; Time Ranges: ");
                IEnumerable<string> timeRangeDescriptions = TimeRanges.Select(timeRange => String.Format("{0} - {1}", timeRange.FromTime.ToShortTimeString(), timeRange.ToTime.ToShortTimeString()));
                description.Append(String.Join(", ", timeRangeDescriptions));
            }

            return description.ToString();
        }
    }
}
