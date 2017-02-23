using System;

namespace Vanrise.AccountBalance.Business
{
    public struct AccountBalanceType
    {
        static AccountBalanceType()
        {
            Vanrise.Common.ProtoBufSerializer.AddSerializableType(typeof(AccountBalanceType), "AccountTypeId", "TransactionTypeId");
        }

        public Guid AccountTypeId { get; set; }
        public Guid TransactionTypeId { get; set; }
    }

    public class BalanceUpdatePayload
    {
        public Guid AccountTypeId { get; set; }
        public Guid TransactionTypeId { get; set; }
        public string AccountId { get; set; }
        public DateTime EffectiveOn { get; set; }
        public Decimal Amount { get; set; }
        public int CurrencyId { get; set; }
    }
}
