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
        public InArgument<EntityType> EntityType { get; set; }

        [RequiredArgument]
        public InArgument<long> EntityId { get; set; }

        [RequiredArgument]
        public InArgument<Guid> StatusDefinitionId { get; set; }

        [RequiredArgument]
        public InArgument<List<Object>> ExecutedActionsData { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            var entityType = this.EntityType.Get(context);
            var entityId = this.EntityId.Get(context);
            var executedActionsData = this.ExecutedActionsData.Get(context);
            var statusDefinitionId =  this.StatusDefinitionId.Get(context);
            switch(entityType)
            {
                case Retail.BusinessEntity.Entities.EntityType.Account:
                    AccountManager accountManager = new AccountManager();
                    accountManager.UpdateStatus(entityId, statusDefinitionId);
                    var account = accountManager.GetAccount(entityId);
                    ExecutedActions accountActions = account.ExecutedActions;
                    if (accountActions == null)
                    {
                        accountActions = new ExecutedActions();
                    }
                    accountActions.ExecutedActionsData = executedActionsData;
                    accountManager.UpdateExecutedActions(entityId, accountActions);
                    break;
                case Retail.BusinessEntity.Entities.EntityType.AccountService:
                    AccountServiceManager accountServiceManager = new AccountServiceManager();
                    accountServiceManager.UpdateStatus(entityId, statusDefinitionId);
                    var accountService = accountServiceManager.GetAccountService(entityId);
                    ExecutedActions serviceActions = accountService.ExecutedActions;
                    if (serviceActions == null)
                    {
                        serviceActions = new ExecutedActions();
                    }
                    accountServiceManager.UpdateExecutedActions(entityId,serviceActions);
                    break;
            }
        }
    }
}
