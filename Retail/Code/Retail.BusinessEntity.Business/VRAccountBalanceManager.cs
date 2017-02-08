using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business.Extensions;
using Vanrise.Notification.Business;
using Vanrise.Common;
using Vanrise.AccountBalance.Entities;

namespace Retail.BusinessEntity.Business
{
    public class VRAccountBalanceManager
    {
        public SubscriberAccountBalanceSetting GetSubscriberAccountBalanceSetting(Guid accountTypeId)
        {
            Vanrise.AccountBalance.Business.AccountTypeManager balanceAccountTypeManager = new Vanrise.AccountBalance.Business.AccountTypeManager();
            Vanrise.AccountBalance.Entities.AccountTypeSettings accountTypeSettings = balanceAccountTypeManager.GetAccountTypeSettings(accountTypeId);
            accountTypeSettings.ThrowIfNull("accountTypeSettings", accountTypeId);
            return accountTypeSettings.ExtendedSettings.CastWithValidate<SubscriberAccountBalanceSetting>("accountTypeSettings.ExtendedSettings");
        }
        public Guid GetAccountBEDefinitionIdByAccountTypeId(Guid accountTypeId)
        {
           var retailAccountBalanceSetting = GetSubscriberAccountBalanceSetting(accountTypeId);
            return retailAccountBalanceSetting.AccountBEDefinitionId;
        }
        public Guid GetAccountBEDefinitionIdByAlertRuleTypeId(Guid alertRuleTypeId)
        {
            VRAlertRuleTypeManager alertRuleTypeManager = new VRAlertRuleTypeManager();
            AccountBalanceAlertRuleTypeSettings balanceRuleTypeSettings = alertRuleTypeManager.GetVRAlertRuleTypeSettings<AccountBalanceAlertRuleTypeSettings>(alertRuleTypeId);
            balanceRuleTypeSettings.ThrowIfNull("balanceRuleTypeSettings", alertRuleTypeId);

            return GetAccountBEDefinitionIdByAccountTypeId(balanceRuleTypeSettings.AccountTypeId);
        }
    }
}
