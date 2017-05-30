using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class DefaultFinancialAccountLocator : FinancialAccountLocator
    {
        public override Guid ConfigId { get { return new Guid("63E3987B-302B-42BA-8E61-A7762FA7BFD3"); } }

        public bool UseFinancialAccountModule { get; set; }

        public override bool TryGetFinancialAccountId(IFinancialAccountLocatorContext context)
        {
            if (this.UseFinancialAccountModule)
            {
                FinancialAccountManager financialAccountManager = new FinancialAccountManager();
                FinancialAccountRuntimeData financialAccountData = financialAccountManager.GetAccountFinancialInfo(context.AccountDefinitionId, context.AccountId, context.EffectiveOn);
                if(financialAccountData != null)
                {
                    context.FinancialAccountId = financialAccountData.FinancialAccountId;
                    context.BalanceAccountTypeId = financialAccountData.BalanceAccountTypeId;
                    context.BalanceAccountId = financialAccountData.BalanceAccountId;
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
