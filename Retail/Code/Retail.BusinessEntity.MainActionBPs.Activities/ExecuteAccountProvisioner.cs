using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.BusinessProcess;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Business;
namespace Retail.BusinessEntity.MainActionBPs.Activities
{
    public sealed class ExecuteAccountProvisioner : Vanrise.BusinessProcess.BaseCodeActivity
    {
        [RequiredArgument]
        public InArgument<long> AccountId { get; set; }
        [RequiredArgument]
        public InArgument<Guid> AccountBEDefinitionId { get; set; }
        [RequiredArgument]
        public InArgument<AccountProvisionerDefinitionSettings> AccountProvisionerDefinition { get; set; }
        [RequiredArgument]
        public InArgument<AccountProvisioner> AccountProvisioner { get; set; }
        [RequiredArgument]
        public InArgument<AccountActionDefinition> AccountActionDefinition { get; set; }

        protected override void VRExecute(IBaseCodeActivityContext context)
        {
            var actionProvisioner = this.AccountProvisioner.Get(context.ActivityContext);
            if (actionProvisioner == null)
                throw new ArgumentNullException("accountProvisioner");
            var definitionSettings = this.AccountProvisionerDefinition.Get(context.ActivityContext);
            var accountId = this.AccountId.Get(context.ActivityContext);
            var accountActionDefinition = this.AccountActionDefinition.Get(context.ActivityContext);
            var accountBEDefinitionId = this.AccountBEDefinitionId.Get(context.ActivityContext);
            var provisioninigContext = new AccountProvisioningContext(this, context.ActivityContext, definitionSettings, accountId, accountBEDefinitionId, accountActionDefinition);
            actionProvisioner.Execute(provisioninigContext);
        }
        private class AccountProvisioningContext : IAccountProvisioningContext
        {
            ExecuteAccountProvisioner _parentActivity;
            ActivityContext _context;
            AccountProvisionerDefinitionSettings _definitionSettings;
            public long _accountId { get; set; }
            public Guid _accountBEDefinitionId { get; set; }
            public AccountActionDefinition _accountActionDefinition { get; set; }
            public AccountProvisioningContext(ExecuteAccountProvisioner parentActivity, ActivityContext context, AccountProvisionerDefinitionSettings definitionSettings, long accountId, Guid accountBEDefinitionId, AccountActionDefinition accountActionDefinition)
            {
                _parentActivity = parentActivity;
                _context = context;
                _definitionSettings = definitionSettings;
                _accountBEDefinitionId = accountBEDefinitionId;
                _accountId = accountId;
                _accountActionDefinition = accountActionDefinition;
            }

            public AccountProvisionerDefinitionSettings DefinitionSettings
            {
                get
                {
                    return _definitionSettings;
                }
            }
            public long AccountId
            {
                get
                {
                    return _accountId;
                }
            }
            public Guid AccountBEDefinitionId
            {
                get
                {
                    return _accountBEDefinitionId;
                }
            }
            public void WriteTrackingMessage(LogEntryType severity, string messageFormat, params object[] args)
            {
                _context.WriteTrackingMessage(severity, messageFormat, args);
            }


            public void WriteBusinessTrackingMsg(LogEntryType severity, string messageFormat, params object[] args)
            {
                _context.WriteBusinessTrackingMsg(severity, messageFormat, args);
            }
            public void TrackActionExecuted(string actionDescription, Object technicalInformation)
            {
                var actionProvisionerTechnicalInformation = new ActionProvisionerTechnicalInformation
                {
                    ActionDefinitionId = _accountActionDefinition.AccountActionDefinitionId,
                    ProvisionerInfo = technicalInformation
                };
                new AccountBEManager().TrackAndLogObjectCustomAction(_accountBEDefinitionId, _accountId, _accountActionDefinition.Name, actionDescription, actionProvisionerTechnicalInformation);
            }
            
  
        }
        public class ActionProvisionerTechnicalInformation
        {
            public Guid ActionDefinitionId { get; set; }
            public Object ProvisionerInfo { get; set; }
        }
    }
}
