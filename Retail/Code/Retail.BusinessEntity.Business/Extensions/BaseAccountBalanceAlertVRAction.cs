using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business.Extensions;
using Vanrise.Notification.Entities;
using Vanrise.Common;
using Retail.BusinessEntity.Entities;

namespace Retail.BusinessEntity.Business
{
    public abstract class BaseAccountBalanceAlertVRAction : VRAction
    {
        static Vanrise.Notification.Business.VRAlertRuleTypeManager s_alertRuleTypeManager = new Vanrise.Notification.Business.VRAlertRuleTypeManager();
        static Vanrise.AccountBalance.Business.AccountTypeManager s_balanceAccountTypeManager = new Vanrise.AccountBalance.Business.AccountTypeManager();
        static AccountBEManager s_accountBEManager = new AccountBEManager();
        protected Guid GetAccountBEDefinitionId(VRBalanceAlertEventPayload balanceAlertEventPayload)
        {
            Vanrise.AccountBalance.Business.Extensions.AccountBalanceAlertRuleTypeSettings balanceRuleTypeSettings = s_alertRuleTypeManager.GetVRAlertRuleTypeSettings<AccountBalanceAlertRuleTypeSettings>(balanceAlertEventPayload.AlertRuleTypeId);
            balanceRuleTypeSettings.ThrowIfNull("balanceRuleTypeSettings", balanceAlertEventPayload.AlertRuleTypeId);
            Vanrise.AccountBalance.Entities.AccountTypeSettings accountTypeSettings = s_balanceAccountTypeManager.GetAccountTypeSettings(balanceRuleTypeSettings.AccountTypeId);
            accountTypeSettings.ThrowIfNull("accountTypeSettings", balanceRuleTypeSettings.AccountTypeId);
            
            SubscriberAccountBalanceSetting retailAccountBalanceSetting = accountTypeSettings.ExtendedSettings.CastWithValidate<SubscriberAccountBalanceSetting>("accountTypeSettings.ExtendedSettings");

            return retailAccountBalanceSetting.AccountBEDefinitionId;
        }

        protected long GetAccountId(VRBalanceAlertEventPayload balanceAlertEventPayload)
        {
            long accountId;
            if (!long.TryParse(balanceAlertEventPayload.EntityId, out accountId))
                throw new Exception(String.Format("balanceAlertEventPayload.EntityId. Cannot parse '{0}' to long", balanceAlertEventPayload.EntityId));
            return accountId;
        }

        protected Account GetAccount(VRBalanceAlertEventPayload balanceAlertEventPayload)
        {
            Guid accountBEDefinitionId = GetAccountBEDefinitionId(balanceAlertEventPayload);
            long accountId = GetAccountId(balanceAlertEventPayload);
            return s_accountBEManager.GetAccount(accountBEDefinitionId, accountId);
        }
    }
}
