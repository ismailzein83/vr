using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountUsageInfo
    {
        public long AccountUsageId { get; set; }
        public String AccountId { get; set; }
        public Guid TransactionTypeId { get; set; }
    }
}
