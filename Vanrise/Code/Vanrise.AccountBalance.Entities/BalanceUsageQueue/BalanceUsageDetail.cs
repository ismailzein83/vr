using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class BalanceUsageDetail
    {
        static BalanceUsageDetail()
        {
            new Entities.UsageBalanceUpdate();
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(BalanceUsageDetail),"TransactionTypeId", "UsageBalanceUpdates");
        }
        public List<UsageBalanceUpdate> UsageBalanceUpdates { get; set; }
        public Guid TransactionTypeId { get; set; }
    }
}
