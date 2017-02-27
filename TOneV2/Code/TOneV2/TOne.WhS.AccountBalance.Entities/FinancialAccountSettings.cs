using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.AccountBalance.Entities
{
    public abstract class FinancialAccountSettings
    {
        public Guid AccountTypeId { get; set; }

        public FinancialAccountExtendedSettings ExtendedSettings { get; set; }
    }

    public abstract class FinancialAccountExtendedSettings
    {
        public abstract bool IsCustomerAccount(IFinancialAccountIsCustomerAccountContext context);

        public abstract bool IsSupplierAccount(IFinancialAccountIsSupplierAccountContext context);
    }

    //public abstract class FinancialAccountExtendedSettings

    public interface IFinancialAccountIsCustomerAccountContext
    {
        Guid DefinitionId { get; }

        Guid UsageTransactionTypeId { set; }

        Decimal? CreditLimit { set; }
    }

    public interface IFinancialAccountIsSupplierAccountContext
    {
        Guid DefinitionId { get; }

        Guid UsageTransactionTypeId { set; }

        Decimal? CreditLimit { set; }
    }
}
