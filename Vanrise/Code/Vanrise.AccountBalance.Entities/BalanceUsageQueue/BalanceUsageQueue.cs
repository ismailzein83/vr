using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class BalanceUsageQueue
    {
        public long BalanceUsageQueueId { get; set; }
        public Guid AccountTypeId { get; set; }
        public BalanceUsageDetail UsageDetails { get; set; }
    }
}
