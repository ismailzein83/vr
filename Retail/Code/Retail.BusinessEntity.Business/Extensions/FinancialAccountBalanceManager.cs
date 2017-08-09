using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business.Extensions;
using Vanrise.AccountBalance.Entities;
using Vanrise.Common;
using Vanrise.Notification.Business;

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
            Vanrise.Entities.VRAccountStatus status =Vanrise.Entities.VRAccountStatus.InActive;
            AccountBEManager accountBEManager = new AccountBEManager();

            if (accountBEManager.IsAccountBalanceActive(financialAccountData.Account))
            {
                status = Vanrise.Entities.VRAccountStatus.Active;
            }
            
            Vanrise.AccountBalance.Entities.AccountInfo accountInfo = new Vanrise.AccountBalance.Entities.AccountInfo
            {
                Name = financialAccountData.Account.Name,
                StatusDescription = new Vanrise.Common.Business.StatusDefinitionManager().GetStatusDefinitionName(financialAccountData.Account.StatusId),
                CurrencyId = s_accountBEManager.GetCurrencyId(_accountBEDefinitionId, financialAccountData.Account),
                BED =financialAccountData.FinancialAccount.BED,
                EED = financialAccountData.FinancialAccount.EED,
                Status = status,
            };
            return accountInfo;
        }


        public static SubscriberAccountBalanceSetting GetSubscriberAccountBalanceSetting(Guid accountTypeId)
        {
            Vanrise.AccountBalance.Business.AccountTypeManager balanceAccountTypeManager = new Vanrise.AccountBalance.Business.AccountTypeManager();
            Vanrise.AccountBalance.Entities.AccountTypeSettings accountTypeSettings = balanceAccountTypeManager.GetAccountTypeSettings(accountTypeId);
            accountTypeSettings.ThrowIfNull("accountTypeSettings", accountTypeId);
            return accountTypeSettings.ExtendedSettings.CastWithValidate<SubscriberAccountBalanceSetting>("accountTypeSettings.ExtendedSettings");
        }
        public static Guid GetAccountBEDefinitionIdByAccountTypeId(Guid accountTypeId)
        {
            var retailAccountBalanceSetting = GetSubscriberAccountBalanceSetting(accountTypeId);
            return retailAccountBalanceSetting.AccountBEDefinitionId;
        }
        public static Guid GetAccountBEDefinitionIdByAlertRuleTypeId(Guid alertRuleTypeId)
        {
            VRAlertRuleTypeManager alertRuleTypeManager = new VRAlertRuleTypeManager();
            AccountBalanceAlertRuleTypeSettings balanceRuleTypeSettings = alertRuleTypeManager.GetVRAlertRuleTypeSettings<AccountBalanceAlertRuleTypeSettings>(alertRuleTypeId);
            balanceRuleTypeSettings.ThrowIfNull("balanceRuleTypeSettings", alertRuleTypeId);

            return GetAccountBEDefinitionIdByAccountTypeId(balanceRuleTypeSettings.AccountTypeId);
        }
    }
}
