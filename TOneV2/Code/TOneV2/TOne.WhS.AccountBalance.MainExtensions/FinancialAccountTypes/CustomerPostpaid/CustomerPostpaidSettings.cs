using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;
using TOne.WhS.AccountBalance.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.FinancialAccountTypes.CustomerPostpaid
{
    public class CustomerPostpaidSettings : FinancialAccountExtendedSettings
    {
        public Decimal CreditLimit { get; set; }

        public override bool IsCustomerAccount(IFinancialAccountIsCustomerAccountContext context)
        {
            CustomerPostpaidDefinitionSettings definitionSetting = new FinancialAccountDefinitionManager().GetFinancialAccountDefinitionExtendedSettings<CustomerPostpaidDefinitionSettings>(context.DefinitionId);
            context.AccountTypeId = definitionSetting.AccountTypeId;
            context.UsageTransactionTypeId = definitionSetting.UsageTransactionTypeId;
            context.CreditLimit = this.CreditLimit;
            return true;
        }

        public override bool IsSupplierAccount(IFinancialAccountIsSupplierAccountContext context)
        {
            return false;
        }
    }
}
