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
        public String AccountId { get; set; }
        public int CurrencyId { get; set; }
        public DateTime? BED { get; set; }
        public DateTime? EED { get; set; }
        public Vanrise.Entities.VRAccountStatus Status { get; set; }
    }
}
