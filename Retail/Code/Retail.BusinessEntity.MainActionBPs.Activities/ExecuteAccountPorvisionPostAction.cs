using Retail.BusinessEntity.Entities;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainActionBPs.Activities
{
    public sealed class ExecuteAccountPorvisionPostAction : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> AccountBEDefinitionId { get; set; }
        [RequiredArgument]
        public InArgument<long> AccountId { get; set; }
        [RequiredArgument]
        public InArgument<AccountProvisionPostAction> ProvisionPostAction { get; set; }
        [RequiredArgument]
        public InArgument<AccountProvisionDefinitionPostAction> ProvisionDefinitionPostAction { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            var accountBEDefinitionId = this.AccountBEDefinitionId.Get(context);
            var accountId = this.AccountId.Get(context);
            var provisionPostAction = this.ProvisionPostAction.Get(context);
            var provisionDefinitionPostAction = this.ProvisionDefinitionPostAction.Get(context);

            provisionPostAction.ExecutePostAction(new AccountProvisionPostActionContext
            {
                AccountBEDefinitionId = accountBEDefinitionId,
                AccountId= accountId,
                DefinitionPostAction = provisionDefinitionPostAction
            });
        }
    }
}
