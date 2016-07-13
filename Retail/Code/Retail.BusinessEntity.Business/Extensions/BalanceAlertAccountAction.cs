using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.AccountBalance.Business;
using Vanrise.Entities;

namespace Retail.BusinessEntity.Business.Extensions
{
    public class BalanceAlertAccountAction : VRAction
    {
        public Guid ActionDefinitionId { get; set; }

        public Vanrise.Integration.Entities.ActionProvisioner ActionProvisioner { get; set; }

        public ActionBPSettings ActionBPSettings { get; set; }

        public override void Execute(IVRActionContext context)
        {
            BalanceAlertEventPayload balanceAlertEventPayload = context.EventPayload as BalanceAlertEventPayload;
            ValidatePayload(balanceAlertEventPayload);
            Account account = balanceAlertEventPayload.Account as Account;
            if(account == null)
                throw new NullReferenceException("account");

            var actionDefinition = GetActionDefinition();                       

            long entityId;
            switch(actionDefinition.Settings.EntityType)
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
                default: throw new NotSupportedException(string.Format("actionDefinition.Settings.EntityType '{0}'", actionDefinition.Settings.EntityType));
            }

            var actionBPInput = new ActionBPInputArgument
            {
                ActionDefinitionId = this.ActionDefinitionId,
                EntityId = entityId,
                ActionProvisioner = this.ActionProvisioner,
                ActionBPSettings = this.ActionBPSettings
            };
            var createProcessInput = new Vanrise.BusinessProcess.Entities.CreateProcessInput { InputArguments = actionBPInput };
            Vanrise.BusinessProcess.Business.BPInstanceManager bpInstanceManager = new Vanrise.BusinessProcess.Business.BPInstanceManager();
            var output = bpInstanceManager.CreateNewProcess(createProcessInput);            
        }

        private static void ValidatePayload(BalanceAlertEventPayload balanceAlertEventPayload)
        {
            if (balanceAlertEventPayload == null)
                throw new NullReferenceException("balanceAlertEventPayload");
            if (balanceAlertEventPayload.Account == null)
                throw new NullReferenceException("balanceAlertEventPayload.Account");
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
