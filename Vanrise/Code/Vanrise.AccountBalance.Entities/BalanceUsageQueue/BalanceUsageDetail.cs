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
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(BalanceUsageDetail), "UsageBalanceUpdates");
        }
        public List<UsageBalanceUpdate> UsageBalanceUpdates { get; set; }
    }
}
