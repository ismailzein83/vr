using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;
using TOne.WhS.AccountBalance.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.FinancialAccountTypes.SupplierPostpaid
{
    public class SupplierPostpaidSettings : FinancialAccountExtendedSettings
    {
        public Decimal CreditLimit { get; set; }

        public override bool IsCustomerAccount(IFinancialAccountIsCustomerAccountContext context)
        {
            return false;
        }

        public override bool IsSupplierAccount(IFinancialAccountIsSupplierAccountContext context)
        {
            SupplierPostpaidDefinitionSettings definitionSetting = new FinancialAccountDefinitionManager().GetFinancialAccountDefinitionExtendedSettings<SupplierPostpaidDefinitionSettings>(context.DefinitionId);
            context.AccountTypeId = definitionSetting.AccountTypeId;
            context.UsageTransactionTypeId = definitionSetting.UsageTransactionTypeId;
            context.CreditLimit = this.CreditLimit;
            return true;
        }
    }
}
