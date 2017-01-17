using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class UpdateUsageBalancePayload
    {
        static UpdateUsageBalancePayload()
        {
            new Entities.UpdateUsageBalanceItem();
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(UpdateUsageBalancePayload), "TransactionTypeId", "UpdateUsageBalanceItems");
        }
        public List<UpdateUsageBalanceItem> UpdateUsageBalanceItems { get; set; }
        public Guid TransactionTypeId { get; set; }
    }
    public class UpdateUsageBalanceItem
    {
        static UpdateUsageBalanceItem()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(UpdateUsageBalanceItem), "AccountId", "Value", "CurrencyId", "EffectiveOn");
        }
        public long AccountId { get; set; }
        public DateTime EffectiveOn { get; set; }
        public Decimal Value { get; set; }
        public int CurrencyId { get; set; }
    }
}
