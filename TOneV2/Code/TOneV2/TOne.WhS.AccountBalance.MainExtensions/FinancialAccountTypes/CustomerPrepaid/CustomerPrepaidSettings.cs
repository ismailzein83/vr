using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;
using TOne.WhS.AccountBalance.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.FinancialAccountTypes.CustomerPrepaid
{
    public class CustomerPrepaidSettings : FinancialAccountExtendedSettings
    {
        public override bool IsCustomerAccount(IFinancialAccountIsCustomerAccountContext context)
        {
            CustomerPrepaidDefinitionSettings definitionSetting = new FinancialAccountDefinitionManager().GetFinancialAccountDefinitionExtendedSettings<CustomerPrepaidDefinitionSettings>(context.DefinitionId);
            context.UsageTransactionTypeId = definitionSetting.UsageTransactionTypeId;
            return true;
        }

        public override bool IsSupplierAccount(IFinancialAccountIsSupplierAccountContext context)
        {
            return false;
        }
    }
}
