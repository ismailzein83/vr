using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;
using TOne.WhS.AccountBalance.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.FinancialAccountTypes.SupplierPrepaid
{
    public class SupplierPrepaidSettings : FinancialAccountExtendedSettings
    {
        public override bool IsCustomerAccount(IFinancialAccountIsCustomerAccountContext context)
        {
            return false;
        }

        public override bool IsSupplierAccount(IFinancialAccountIsSupplierAccountContext context)
        {
            SupplierPrepaidDefinitionSettings definitionSetting = new FinancialAccountDefinitionManager().GetFinancialAccountDefinitionExtendedSettings<SupplierPrepaidDefinitionSettings>(context.DefinitionId);
            context.AccountTypeId = definitionSetting.AccountTypeId;
            context.UsageTransactionTypeId = definitionSetting.UsageTransactionTypeId;
            return true;
        }
    }
}
