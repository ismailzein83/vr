using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class UsageBalanceUpdate
    {
        static UsageBalanceUpdate()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(UsageBalanceUpdate),  "AccountId", "Value", "CurrencyId", "EffectiveOn");
        }
        public long AccountId { get; set; }
        public DateTime EffectiveOn { get; set; }
        public Decimal Value { get; set; }
        public int CurrencyId { get; set; }
    }
}
