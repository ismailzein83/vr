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
        public Guid TransactionTypeId { get; set; }
        public String AccountId { get; set; }
        public DateTime PeriodStart { get; set; }
        public DateTime PeriodEnd { get; set; }
        public decimal UsageBalance { get; set; }
        public int CurrencyId { get; set; }
        public bool IsOverriden { get; set; }
        public decimal? OverridenAmount { get; set; }
        public Guid? CorrectionProcessId { get; set; }
    }
}
