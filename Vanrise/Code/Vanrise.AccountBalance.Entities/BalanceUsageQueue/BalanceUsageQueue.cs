using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public enum BalanceUsageQueueType { UpdateUsageBalance = 0, CorrectUsageBalance = 1 }
    public class BalanceUsageQueue<T>
    {
        public long BalanceUsageQueueId { get; set; }
        public Guid AccountTypeId { get; set; }
        public BalanceUsageQueueType BalanceUsageQueueType { get; set; }
        public T UsageDetails { get; set; }
    }
}
