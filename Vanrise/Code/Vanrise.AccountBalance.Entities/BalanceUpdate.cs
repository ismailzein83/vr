using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class BalanceUpdate
    {
        public long AccountId { get; set; }

        public Decimal Value { get; set; }
    }
}
