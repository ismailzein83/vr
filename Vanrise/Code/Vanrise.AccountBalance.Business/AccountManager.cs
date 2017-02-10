using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common;

namespace Vanrise.AccountBalance.Business
{
    public class AccountManager
    {
        AccountTypeManager _accountTypeManager = new AccountTypeManager();
        GenericData.Business.BusinessEntityManager _businessEntityManager = new GenericData.Business.BusinessEntityManager();
        public dynamic GetAccount(Guid accountTypeId, String accountId)
        {
            var accountManager = _accountTypeManager.GetAccountManager(accountTypeId);
            AccountContext context = new AccountContext
            {
                AccountId = accountId
            };
            return accountManager.GetAccount(context);
        }
        public AccountInfo GetAccountInfo(Guid accountTypeId, String accountId)
        {
           var accountManager = _accountTypeManager.GetAccountManager(accountTypeId);
           AccountInfoContext context = new AccountInfoContext
           {
               AccountId = accountId
           };
           return accountManager.GetAccountInfo(context);
        }

        public string GetAccountName(Guid accountTypeId, String accountId)
        {
            var accountInfo = GetAccountInfo(accountTypeId, accountId);
            accountInfo.ThrowIfNull("accountInfo", String.Format("{0} {1}", accountTypeId, accountId));
            return accountInfo.Name;
        }
    }
}
