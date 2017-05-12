using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Retail.BusinessEntity.Business
{
    public class FinancialAccountBalanceManager: IAccountManager
    {
        Guid _accountBEDefinitionId;
        public FinancialAccountBalanceManager(Guid accountBEDefinitionId)
        {
            _accountBEDefinitionId = accountBEDefinitionId;
        }

        public dynamic GetAccount(IAccountContext context) 
        {
            AccountBEManager accountBEManager = new AccountBEManager();
            FinancialAccountManager manager = new FinancialAccountManager();
            var financialAccountData = manager.GetFinancialAccountData(this._accountBEDefinitionId, context.AccountId);
            return financialAccountData.Account;
        }
        public Vanrise.AccountBalance.Entities.AccountInfo GetAccountInfo(IAccountInfoContext context)
        {
            FinancialAccountManager manager = new FinancialAccountManager();
            var financialAccountData = manager.GetFinancialAccountData(this._accountBEDefinitionId, context.AccountId);
            Vanrise.AccountBalance.Entities.AccountInfo accountInfo = new Vanrise.AccountBalance.Entities.AccountInfo
            {
                Name = financialAccountData.Account.Name,
                StatusDescription = new StatusDefinitionManager().GetStatusDefinitionName(financialAccountData.Account.StatusId),
            };
            var currency = GetCurrencyId(financialAccountData.Account.Settings.Parts.Values);
            if (currency.HasValue)
            {
                accountInfo.CurrencyId = currency.Value;
            }
            else
            {
                throw new Exception(string.Format("Account {0} does not have currency", accountInfo.Name));
            }
            return accountInfo;
        }
        private int? GetCurrencyId(IEnumerable<AccountPart> parts)
        {
            foreach (AccountPart part in parts)
            {
                var actionpartSetting = part.Settings as IAccountPayment;
                if (actionpartSetting != null)
                {
                    return actionpartSetting.CurrencyId;
                }
            }
            return null;
        }
    }
}
