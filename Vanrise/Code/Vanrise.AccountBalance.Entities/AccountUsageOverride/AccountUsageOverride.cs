using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountUsageOverride
    {
        public Guid AccountTypeId { get; set; }

        public string AccountId { get; set; }

        public Guid TransactionTypeId { get; set; }

        public DateTime PeriodStart { get; set; }

        public DateTime PeriodEnd { get; set; }

        public long OverriddenByTransactionId { get; set; }
    }
}
