using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class UsageBalanceUpdate
    {
        public long AccountId { get; set; }

        public Decimal Value { get; set; }
    }
}
