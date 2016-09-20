using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.Notification.Entities;

namespace Retail.BusinessEntity.Business.Extensions
{
    public class BalanceAlertAccountAction : VRAction
    {
        public override Guid ConfigId { get { return new Guid("a8e093f4-f9c3-420b-99b7-32eea2c1df78"); } }

        public Guid ActionDefinitionId { get; set; }

        public ActionBPSettings ActionBPSettings { get; set; }

        public override void Execute(IVRActionExecutionContext context)
        {
            BalanceAlertEventPayload balanceAlertEventPayload = context.EventPayload as BalanceAlertEventPayload;
            if (balanceAlertEventPayload == null)
                throw new NullReferenceException("balanceAlertEventPayload");
            Account account = new AccountManager().GetAccount(balanceAlertEventPayload.AccountId);
            if(account == null)
                throw new NullReferenceException("account");

            var actionDefinition = GetActionDefinition();                       

            long entityId;
            switch(actionDefinition.EntityType)
            {
                case Entities.EntityType.Account: entityId = account.AccountId; break;
                case Entities.EntityType.AccountService:
                    if (!actionDefinition.Settings.EntityTypeId.HasValue)
                        throw new NullReferenceException(String.Format("actionDefinition.Settings.EntityTypeId. actionDefinitionId '{0}'", this.ActionDefinitionId));
                    var accountService = new AccountServiceManager().GetAccountService(account.AccountId, actionDefinition.Settings.EntityTypeId.Value);
                    if (accountService == null)//service type is not assigned to account
                        return;
                    entityId = accountService.AccountServiceId;
                    break;
                default: throw new NotSupportedException(string.Format("actionDefinition.Settings.EntityType '{0}'", actionDefinition.EntityType));
            }

            var actionBPInput = new ActionBPInputArgument
            {
                ActionDefinitionId = this.ActionDefinitionId,
                ActionEntityId = entityId,
                ActionBPSettings = this.ActionBPSettings
            };
            var createProcessInput = new Vanrise.BusinessProcess.Entities.CreateProcessInput { InputArguments = actionBPInput };
            createProcessInput.InputArguments.UserId = context.UserID;
            Vanrise.BusinessProcess.Business.BPInstanceManager bpInstanceManager = new Vanrise.BusinessProcess.Business.BPInstanceManager();
            var output = bpInstanceManager.CreateNewProcess(createProcessInput);            
        }

        private ActionDefinition GetActionDefinition()
        {
            var actionDefinition = new ActionDefinitionManager().GetActionDefinition(this.ActionDefinitionId);
            if (actionDefinition == null)
                throw new NullReferenceException(String.Format("actionDefinition '{0}'", this.ActionDefinitionId));
            if (actionDefinition.Settings == null)
                throw new NullReferenceException(String.Format("actionDefinition.Settings '{0}'", this.ActionDefinitionId));
            return actionDefinition;
        }
    }
}
