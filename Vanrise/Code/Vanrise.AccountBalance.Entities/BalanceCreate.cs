using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class BalanceCreate
    {
        public long AccountId { get; set; }

        public Decimal InitialBalance { get; set; }
    }
}
