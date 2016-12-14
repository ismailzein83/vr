using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public abstract class AccountUsagePeriodSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract void EvaluatePeriod(IAccountUsagePeriodEvaluationContext context);
    }

    public interface IAccountUsagePeriodEvaluationContext
    {
        DateTime UsageTime { get; }

        DateTime PeriodStart { set; }

        DateTime PeriodEnd { set; }
    }
}
