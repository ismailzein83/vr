﻿using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class FinancialAccountBalanceManager: IAccountManager
    {
        Guid _accountBEDefinitionId;
        static AccountBEManager s_accountBEManager = new AccountBEManager();
        public FinancialAccountBalanceManager(Guid accountBEDefinitionId)
        {
            _accountBEDefinitionId = accountBEDefinitionId;
        }

        public dynamic GetAccount(IAccountContext context) 
        {
            
            FinancialAccountManager manager = new FinancialAccountManager();
            var financialAccountData = manager.GetFinancialAccountData(this._accountBEDefinitionId, context.AccountId);
            return financialAccountData.Account;
        }
        public Vanrise.AccountBalance.Entities.AccountInfo GetAccountInfo(IAccountInfoContext context)
        {
            FinancialAccountManager manager = new FinancialAccountManager();
            var financialAccountData = manager.GetFinancialAccountData(this._accountBEDefinitionId, context.AccountId);
            financialAccountData.ThrowIfNull("financialAccountData", context.AccountId);
            financialAccountData.Account.ThrowIfNull("financialAccountData.Account", context.AccountId);
            Vanrise.AccountBalance.Entities.AccountInfo accountInfo = new Vanrise.AccountBalance.Entities.AccountInfo
            {
                Name = financialAccountData.Account.Name,
                StatusDescription = new StatusDefinitionManager().GetStatusDefinitionName(financialAccountData.Account.StatusId),
                CurrencyId = s_accountBEManager.GetCurrencyId(_accountBEDefinitionId, financialAccountData.Account)
            };
            return accountInfo;
        }
    }
}
