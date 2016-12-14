using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.MainExtensions.AccountUsagePeriods
{
    public class DailyAccountUsagePeriod : AccountUsagePeriodSettings
    {
        public override void EvaluatePeriod(IAccountUsagePeriodEvaluationContext context)
        {
            DateTime periodStart = context.UsageTime.Date;
            context.PeriodStart = periodStart;
            context.PeriodEnd = periodStart.AddDays(1);
        }

        public override Guid ConfigId
        {
            get { return new Guid("F0A06C27-2850-40ED-BF6C-F0E65BD3894D"); }
        }
    }
}
