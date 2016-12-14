using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountUsage
    {
        public long AccountUsageId { get; set; }
        public Guid AccountTypeId { get; set; }
        public long AccountId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal UsageBalance { get; set; }
        public long BillingTransactionId { get; set; }
        public bool ShouldRecreateTransaction { get; set; }
    }
}
