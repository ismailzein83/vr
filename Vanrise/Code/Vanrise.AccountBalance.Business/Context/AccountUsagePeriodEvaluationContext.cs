using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class AccountUsagePeriodEvaluationContext : IAccountUsagePeriodEvaluationContext
    {
        public DateTime UsageTime { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
    }
}
