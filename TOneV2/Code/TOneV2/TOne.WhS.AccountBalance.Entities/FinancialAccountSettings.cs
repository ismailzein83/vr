using System;

namespace TOne.WhS.AccountBalance.Entities
{
    public class FinancialAccountSettings
    {
        public Guid AccountTypeId { get; set; }

        public FinancialAccountExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class FinancialAccountExtendedSettings
    {
        public abstract bool IsCustomerAccount(IFinancialAccountIsCustomerAccountContext context);

        public abstract bool IsSupplierAccount(IFinancialAccountIsSupplierAccountContext context);
    }
}