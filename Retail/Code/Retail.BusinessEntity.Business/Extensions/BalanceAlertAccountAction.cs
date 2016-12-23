using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.Notification.Entities;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business.Extensions
{
    public class BalanceAlertAccountAction : VRAction
    {
        public override Guid ConfigId { get { return new Guid("a8e093f4-f9c3-420b-99b7-32eea2c1df78"); } }

        public Guid ActionDefinitionId { get; set; }

        public ActionBPSettings ActionBPSettings { get; set; }

        public override bool TryConvertToBPInputArgument(IVRActionConvertToBPInputArgumentContext context)
        {
            VRBalanceAlertEventPayload balanceAlertEventPayload = context.EventPayload as VRBalanceAlertEventPayload;
            balanceAlertEventPayload.ThrowIfNull("balanceAlertEventPayload", "");

            long accountId = 0;
            long.TryParse(balanceAlertEventPayload.EntityId, out accountId);
            Account account = new AccountManager().GetAccount(accountId);
            account.ThrowIfNull("account", accountId);

            var actionDefinition = GetActionDefinition();

            long entityId;
            switch (actionDefinition.EntityType)
            {
                case Entities.EntityType.Account: entityId = account.AccountId; break;
                case Entities.EntityType.AccountService:
                    if (!actionDefinition.Settings.EntityTypeId.HasValue)
                        throw new NullReferenceException(String.Format("actionDefinition.Settings.EntityTypeId. actionDefinitionId '{0}'", this.ActionDefinitionId));
                    var accountService = new AccountServiceManager().GetAccountService(account.AccountId, actionDefinition.Settings.EntityTypeId.Value);
                    if (accountService == null)//service type is not assigned to account
                    {
                        return false;
                    }
                    entityId = accountService.AccountServiceId;
                    break;
                default: throw new NotSupportedException(string.Format("actionDefinition.Settings.EntityType '{0}'", actionDefinition.EntityType));
            }

            context.BPInputArgument = new ActionBPInputArgument
            {
                ActionDefinitionId = this.ActionDefinitionId,
                ActionEntityId = entityId,
                ActionBPSettings = this.ActionBPSettings
            };
            return true;
        }

        private ActionDefinition GetActionDefinition()
        {
            var actionDefinition = new ActionDefinitionManager().GetActionDefinition(this.ActionDefinitionId);

            actionDefinition.ThrowIfNull("actionDefinition", this.ActionDefinitionId);
            actionDefinition.Settings.ThrowIfNull("actionDefinition.Settings", this.ActionDefinitionId);

            return actionDefinition;
        }
    }
}
