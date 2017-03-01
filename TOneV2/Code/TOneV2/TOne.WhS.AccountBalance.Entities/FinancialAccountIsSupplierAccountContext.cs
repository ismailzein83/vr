using System;

namespace TOne.WhS.AccountBalance.Entities
{
    public interface IFinancialAccountIsSupplierAccountContext
    {
        Guid AccountTypeId { get; }

        Guid UsageTransactionTypeId { set; }

        Decimal? CreditLimit { set; }
    }

    public class FinancialAccountIsSupplierAccountContext : IFinancialAccountIsSupplierAccountContext
    {
        public Guid AccountTypeId { get; set; }

        public Guid UsageTransactionTypeId { get; set; }

        public Decimal? CreditLimit { get; set; }
    }
}