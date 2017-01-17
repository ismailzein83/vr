using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.AccountBalance.Entities
{
    public class CorrectUsageBalancePayload
    {
        static CorrectUsageBalancePayload()
        {
            new Entities.CorrectUsageBalanceItem();
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CorrectUsageBalancePayload), "TransactionTypeId", "CorrectUsageBalanceItems", "PeriodDate");
        }
        public List<CorrectUsageBalanceItem> CorrectUsageBalanceItems { get; set; }
        public Guid TransactionTypeId { get; set; }
        public DateTime PeriodDate { get; set; }
        public bool IsLastBatch { get; set; }
    }
    public class CorrectUsageBalanceItem
    {
        public long AccountId { get; set; }
        public Decimal Value { get; set; }
        public int CurrencyId { get; set; }
    }
}
