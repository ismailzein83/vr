using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.AccountBalance.Business;
using TOne.WhS.AccountBalance.Entities;

namespace TOne.WhS.AccountBalance.MainExtensions.FinancialAccountTypes.Netting
{
    public class NettingSettings : FinancialAccountExtendedSettings
    {
        public Decimal CustomerCreditLimit { get; set; }

        public Decimal SupplierCreditLimit { get; set; }

        public override bool IsCustomerAccount(IFinancialAccountIsCustomerAccountContext context)
        {
            NettingDefinitionSettings definitionSetting = new FinancialAccountDefinitionManager().GetFinancialAccountDefinitionExtendedSettings<NettingDefinitionSettings>(context.AccountTypeId);
            context.UsageTransactionTypeId = definitionSetting.CustomerUsageTransactionTypeId;
            context.CreditLimit = this.CustomerCreditLimit;
            return true;
        }

        public override bool IsSupplierAccount(IFinancialAccountIsSupplierAccountContext context)
        {

            NettingDefinitionSettings definitionSetting = new FinancialAccountDefinitionManager().GetFinancialAccountDefinitionExtendedSettings<NettingDefinitionSettings>(context.AccountTypeId);
            context.UsageTransactionTypeId = definitionSetting.SupplierUsageTransactionTypeId;
            context.CreditLimit = this.SupplierCreditLimit;
            return true;
        }
    }
}
