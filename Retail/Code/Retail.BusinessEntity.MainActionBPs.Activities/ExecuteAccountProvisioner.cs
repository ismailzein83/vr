using Retail.BusinessEntity.Business.AccountProvisioning;
using Retail.BusinessEntity.Entities;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

            var provisioninigContext = new AccountProvisioningContext
            {
                DefinitionSettings = this.AccountProvisionerDefinition.Get(context),
                AccountId = this.AccountId.Get(context),
                AccountBEDefinitionId = this.AccountBEDefinitionId.Get(context)
            };
            actionProvisioner.Execute(provisioninigContext);
        }
    }
}
