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
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CorrectUsageBalancePayload), "TransactionTypeId", "CorrectUsageBalanceItems", "PeriodDate", "IsLastBatch", "CorrectionProcessId");
        }
        public List<CorrectUsageBalanceItem> CorrectUsageBalanceItems { get; set; }
        public Guid TransactionTypeId { get; set; }
        public DateTime PeriodDate { get; set; }
        public bool IsLastBatch { get; set; }
        public Guid CorrectionProcessId { get; set; }
    }
    public class CorrectUsageBalanceItem
    {
        static CorrectUsageBalanceItem()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(CorrectUsageBalanceItem), "AccountId", "Value", "CurrencyId");
        }
        public long AccountId { get; set; }
        public Decimal Value { get; set; }
        public int CurrencyId { get; set; }
    }
}
