using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class SubscriberAccountBalanceManager : IAccountManager
    {
        Guid _accountBEDefinitionId;
        static AccountBEManager s_accountBEManager = new AccountBEManager();
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
            account.ThrowIfNull("account", context.AccountId);
            Vanrise.AccountBalance.Entities.AccountInfo accountInfo = new Vanrise.AccountBalance.Entities.AccountInfo
            {
                Name = account.Name,
                StatusDescription = new StatusDefinitionManager().GetStatusDefinitionName(account.StatusId),
                CurrencyId = s_accountBEManager.GetCurrencyId(_accountBEDefinitionId, account),
                BED  = null,
                EED = null,
                IsDeleted = false,
                Status = Vanrise.Entities.VRAccountStatus.Active
            };
            return accountInfo;
        }
    }
}
