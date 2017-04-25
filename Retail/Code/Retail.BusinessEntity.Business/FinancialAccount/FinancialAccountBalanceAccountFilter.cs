using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.Business
{
    public class FinancialAccountBalanceAccountFilter : IAccountFilter
    {
        public Guid AccountTypeId { get; set; }

        static FinancialAccountManager s_financialAccountManager = new FinancialAccountManager();
        static FinancialAccountDefinitionManager s_financialAccountDefinitionManager = new FinancialAccountDefinitionManager();

        public bool IsExcluded(IAccountFilterContext context)
        {
            FinancialAccountData financialAccountData;
            if(s_financialAccountManager.TryGetFinancialAccount(context.AccountBEDefinitionId, context.Account.AccountId, false, DateTime.Now, out financialAccountData))
            {
                var financialAccountDefinitionSettings = s_financialAccountDefinitionManager.GetFinancialAccountDefinitionSettings(financialAccountData.FinancialAccount.FinancialAccountDefinitionId);
                if (financialAccountDefinitionSettings.BalanceAccountTypeId.HasValue && financialAccountDefinitionSettings.BalanceAccountTypeId.Value == this.AccountTypeId)
                    return false;//dont exclude
                else
                    return true;
            }
            else
            {
                return true;
            }
        }
    }
}
