using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities;
using Vanrise.BusinessProcess;
using Retail.BusinessEntity.Entities;
namespace Retail.BusinessEntity.MainActionBPs.Activities
{
    public sealed class ExecuteAccountProvisioner : NativeActivity
    {
        [RequiredArgument]
        public InArgument<long> AccountId { get; set; }
        [RequiredArgument]
        public InArgument<Guid> AccountBEDefinitionId { get; set; }
        [RequiredArgument]
        public InArgument<AccountProvisionerDefinitionSettings> AccountProvisionerDefinition { get; set; }
        [RequiredArgument]
        public InArgument<AccountProvisioner> AccountProvisioner { get; set; }
        protected override void Execute(NativeActivityContext context)
        {
            var actionProvisioner = this.AccountProvisioner.Get(context);
            if (actionProvisioner == null)
                throw new ArgumentNullException("accountProvisioner");
            var definitionSettings = this.AccountProvisionerDefinition.Get(context);
            var accountId = this.AccountId.Get(context);
            var accountBEDefinitionId = this.AccountBEDefinitionId.Get(context);
            var provisioninigContext = new AccountProvisioningContext(this, context, definitionSettings, accountId, accountBEDefinitionId);
            actionProvisioner.Execute(provisioninigContext);
        }

        private class AccountProvisioningContext : IAccountProvisioningContext
        {
            ExecuteAccountProvisioner _parentActivity;
            NativeActivityContext _context;
            AccountProvisionerDefinitionSettings _definitionSettings;
            public long _accountId { get; set; }
            public Guid _accountBEDefinitionId { get; set; }
            public AccountProvisioningContext(ExecuteAccountProvisioner parentActivity, NativeActivityContext context, AccountProvisionerDefinitionSettings definitionSettings, long accountId, Guid accountBEDefinitionId)
            {
                _parentActivity = parentActivity;
                _context = context;
                _definitionSettings = definitionSettings;
                _accountBEDefinitionId = accountBEDefinitionId;
                _accountId = accountId;
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
        }
    }
}
