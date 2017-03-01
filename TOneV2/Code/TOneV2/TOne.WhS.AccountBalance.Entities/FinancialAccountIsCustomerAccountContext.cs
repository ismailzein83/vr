using System;

namespace TOne.WhS.AccountBalance.Entities
{
    public interface IFinancialAccountIsCustomerAccountContext
    {
        Guid AccountTypeId { get; }

        Guid UsageTransactionTypeId { set; }

        Decimal? CreditLimit { set; }
    }

    public class FinancialAccountIsCustomerAccountContext : IFinancialAccountIsCustomerAccountContext
    {
        public Guid AccountTypeId { get; set; }

        public Guid UsageTransactionTypeId { get; set; }

        public Decimal? CreditLimit { get; set; }
    }
}