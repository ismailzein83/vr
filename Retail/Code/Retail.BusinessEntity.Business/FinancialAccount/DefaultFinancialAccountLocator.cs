using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class DefaultFinancialAccountLocator : FinancialAccountLocator
    {
        public override Guid ConfigId { get { return new Guid("63E3987B-302B-42BA-8E61-A7762FA7BFD3"); } }

        public bool UseFinancialAccountModule { get; set; }

        static FinancialAccountDefinitionManager s_financialAccountDefinitionManager = new FinancialAccountDefinitionManager();

        public override bool TryGetFinancialAccountId(IFinancialAccountLocatorContext context)
        {
            if (this.UseFinancialAccountModule)
            {
                FinancialAccountManager financialAccountManager = new FinancialAccountManager();
                FinancialAccountData financialAccountData;
                if(financialAccountManager.TryGetFinancialAccount(context.AccountDefinitionId, context.AccountId, true, context.EffectiveOn,context.Classification, out financialAccountData))
                {
                    context.FinancialAccountId = financialAccountData.Account.AccountId;
                    var financialAccountDefinitionSettings = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccountData.FinancialAccount.FinancialAccountDefinitionId);
                    financialAccountDefinitionSettings.ThrowIfNull("financialAccountDefinitionSettings", financialAccountData.FinancialAccount.FinancialAccountDefinitionId);
                    if (financialAccountDefinitionSettings.BalanceAccountTypeId.HasValue)
                    {
                        context.BalanceAccountTypeId = financialAccountDefinitionSettings.BalanceAccountTypeId.Value;
                        context.BalanceAccountId = financialAccountData.FinancialAccountId;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                long? financialAccountId = new AccountBEManager().GetFinancialAccountId(context.AccountDefinitionId, context.AccountId);
                if (!financialAccountId.HasValue)
                    return false;

                context.FinancialAccountId = financialAccountId.Value;
                context.BalanceAccountId = financialAccountId.Value.ToString();
                context.BalanceAccountTypeId = new AccountBalanceManager().GetAccountBalanceTypeId(context.AccountDefinitionId);
                return true;
            }
        }
    }
}
