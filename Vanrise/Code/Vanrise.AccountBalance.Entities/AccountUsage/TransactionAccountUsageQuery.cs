using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class TransactionAccountUsageQuery
    {
        public long TransactionId { get; set; }

        public Guid TransactionTypeId { get; set; }

        public Guid AccountTypeId { get; set; }

        public string AccountId { get; set; }

        public DateTime PeriodStart { get; set; }

        public DateTime PeriodEnd { get; set; }
    }
}
