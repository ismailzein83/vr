using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.MainExtensions.AccountUsagePeriods
{
    public class ThirtyMinAccountUsagePeriod : AccountUsagePeriodSettings
    {
        public override Guid ConfigId { get { return new Guid("7DF9BEA6-6DCA-41DD-8C9A-A9D8AD323512"); } }

        public override void EvaluatePeriod(IAccountUsagePeriodEvaluationContext context)
        {
            int timeRoundingIntervalInMinutes = 30;
            DateTime usageTime = context.UsageTime;

            int minuteNumber = usageTime.Hour * 60 + usageTime.Minute;
            int roundedMinuteNumber = (((int)minuteNumber / timeRoundingIntervalInMinutes) * timeRoundingIntervalInMinutes);
            TimeSpan ts = TimeSpan.FromMinutes(roundedMinuteNumber);

            DateTime roundedUsageTime = new DateTime(usageTime.Year, usageTime.Month, usageTime.Day, ts.Hours, ts.Minutes, 0, 0);

            context.PeriodStart = roundedUsageTime;
            context.PeriodEnd = roundedUsageTime.AddMinutes(30);
        }
    }
}
