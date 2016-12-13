using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class LiveBalanceAccountInfo
    {
        public long LiveBalanceId { get; set; }
        public long AccountId { get; set; }
        public int CurrencyId { get; set; }
    }
}
