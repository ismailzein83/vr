using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class AccountBalanceBatch
    {
        public List<LiveBalance> AccountBalances { get; set; }
    }
}
