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
        static AccountBEManager s_accountBEManager = new AccountBEManager();
        static VRAccountBalanceManager s_vRAccountBalanceManager = new VRAccountBalanceManager();
        protected Guid GetAccountBEDefinitionId(VRBalanceAlertEventPayload balanceAlertEventPayload)
        {
            return s_vRAccountBalanceManager.GetAccountBEDefinitionIdByAlertRuleTypeId(balanceAlertEventPayload.AlertRuleTypeId);
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
