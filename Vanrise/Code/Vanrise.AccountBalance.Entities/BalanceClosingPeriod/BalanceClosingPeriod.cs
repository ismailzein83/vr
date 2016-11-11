using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class BalanceClosingPeriod
    {
        public long BalanceClosingPeriodId { get; set; }
        public DateTime ClosingTime { get; set; }
        public Guid AccountTypeId { get; set; }
    }
}
