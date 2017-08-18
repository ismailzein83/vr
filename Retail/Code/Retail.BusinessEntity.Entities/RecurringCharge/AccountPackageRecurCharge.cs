using System;

namespace Retail.BusinessEntity.Entities
{
    public struct AccountPackageRecurChargeKey
    {
        public Guid? BalanceAccountTypeID { get; set; }
        public Guid TransactionTypeId { get; set; }
        public DateTime ChargeDay { get; set; }
    }

    public class AccountPackageRecurCharge
    {
        public long AccountPackageRecurChargeId { get; set; }
        public int AccountPackageID { get; set; }
        public Guid ChargeableEntityID { get; set; }
        public Guid? BalanceAccountTypeID { get; set; }
        public string BalanceAccountID { get; set; }
        public Guid AccountBEDefinitionId { get; set; }
        public long AccountID { get; set; }
        public DateTime ChargeDay { get; set; }
        public decimal ChargeAmount { get; set; }
        public int CurrencyID { get; set; }
        public Guid TransactionTypeID { get; set; }
        public long ProcessInstanceID { get; set; }
        public bool IsSentToAccountBalance { get; set; }
        public DateTime CreatedTime { get; set; }
    }
}