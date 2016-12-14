﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;

namespace Vanrise.AccountBalance.Business
{
    public class AccountManager
    {
        public dynamic GetAccount(long accountId)
        {
            Guid accountBEDefinitionId = new ConfigurationManager().GetAccountBEDefinitionId();
            return new GenericData.Business.BusinessEntityManager().GetEntity(accountBEDefinitionId, accountId);
        }

        AccountTypeManager _accountTypeManager = new AccountTypeManager();
        GenericData.Business.BusinessEntityManager _businessEntityManager = new GenericData.Business.BusinessEntityManager();
        public dynamic GetAccount(Guid accountTypeId, long accountId)
        {
            var accountManager = _accountTypeManager.GetAccountManager(accountTypeId);
            AccountContext context = new AccountContext
            {
                AccountId = accountId
            };
            return accountManager.GetAccount(context);
        }
        public AccountInfo GetAccountInfo(Guid accountTypeId, long accountId)
        {
           var accountManager = _accountTypeManager.GetAccountManager(accountTypeId);
           AccountInfoContext context = new AccountInfoContext
           {
               AccountId = accountId
           };
           return accountManager.GetAccountInfo(context);
        }
    }
}
