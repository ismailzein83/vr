using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Retail.BusinessEntity.Entities;
using Retail.BusinessEntity.Business;

namespace Retail.BusinessEntity.MainActionBPs.Activities
{

    public sealed class UpdateEntityStatus : CodeActivity
    {
        public InArgument<Guid> ActionDefinitionId { get; set; }
        [RequiredArgument]
        public InArgument<Guid> AccountBEDefinitionId { get; set; }
        [RequiredArgument]
        public InArgument<long> AccountId { get; set; }
        [RequiredArgument]
        public InArgument<Guid> StatusDefinitionId { get; set; }

        [RequiredArgument]
        public InArgument<List<Object>> ExecutedActionsData { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            var accountBEDefinitionId = this.AccountBEDefinitionId.Get(context);
            var accountId = this.AccountId.Get(context);
            var executedActionsData = this.ExecutedActionsData.Get(context);
            var statusDefinitionId =  this.StatusDefinitionId.Get(context);
            AccountManager accountManager = new AccountManager();
            AccountBEManager accountBEManager = new AccountBEManager();
            accountManager.UpdateStatus( accountId, statusDefinitionId);
            var account = accountBEManager.GetAccount(accountBEDefinitionId,accountId);
            ExecutedActions accountActions = account.ExecutedActions;
            if (accountActions == null)
            {
                accountActions = new ExecutedActions();
            }
            accountActions.ExecutedActionsData = executedActionsData;
            accountManager.UpdateExecutedActions(accountId, accountActions);
        }
    }
}
