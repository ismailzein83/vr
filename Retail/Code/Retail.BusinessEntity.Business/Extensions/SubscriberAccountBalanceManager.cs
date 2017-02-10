using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Retail.BusinessEntity.Business
{
    public class SubscriberAccountBalanceManager : IAccountManager
    {
        Guid _accountBEDefinitionId;
        public SubscriberAccountBalanceManager(Guid accountBEDefinitionId)
        {
            _accountBEDefinitionId = accountBEDefinitionId;
        }

        public dynamic GetAccount(IAccountContext context) 
        {
            AccountBEManager accountBEManager = new AccountBEManager();
            return accountBEManager.GetAccount(this._accountBEDefinitionId,Convert.ToInt64(context.AccountId));
        }
        public Vanrise.AccountBalance.Entities.AccountInfo GetAccountInfo(IAccountInfoContext context)
        {
            AccountBEManager accountBEManager = new AccountBEManager();
            var account = accountBEManager.GetAccount(this._accountBEDefinitionId, Convert.ToInt64(context.AccountId));
            Vanrise.AccountBalance.Entities.AccountInfo accountInfo = new Vanrise.AccountBalance.Entities.AccountInfo
            {
                Name = account.Name,
                StatusDescription = new StatusDefinitionManager().GetStatusDefinitionName(account.StatusId),
            };
             var currency = GetCurrencyId(account.Settings.Parts.Values);
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
