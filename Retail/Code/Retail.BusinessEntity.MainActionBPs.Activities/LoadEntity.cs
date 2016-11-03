using Retail.BusinessEntity.Business;
using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainActionBPs.Activities
{
    public sealed class LoadEntity : CodeActivity
    {
        public InArgument<Guid> ActionDefinitionId { get; set; }
        public InArgument<string> EntityId { get; set; }
        public OutArgument<dynamic> Entity { get; set; }
        public OutArgument<List<Object>> ExecutedActionsData { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            ActionDefinitionManager actionDefinitionManager = new ActionDefinitionManager();
            var actionDefinition = actionDefinitionManager.GetActionDefinition(this.ActionDefinitionId.Get(context));
            switch(actionDefinition.EntityType)
            {
                case BusinessEntity.Entities.EntityType.Account:
                    {
                        var arr = this.EntityId.Get(context).Split('_');
                        AccountManager accountManager = new AccountManager();
                        var account = accountManager.GetAccount(Convert.ToInt64(arr[arr.Length-1]));
                        this.Entity.Set(context, account);
                        if (account.ExecutedActions != null)
                            this.ExecutedActionsData.Set(context, account.ExecutedActions.ExecutedActionsData);
                    }
                    break;
                case BusinessEntity.Entities.EntityType.AccountService:
                    {
                        var arr = this.EntityId.Get(context).Split('_');
                        AccountServiceManager accountServiceManager = new AccountServiceManager();
                        var accountService = accountServiceManager.GetAccountService(Convert.ToInt64(arr[arr.Length - 1]));
                        this.Entity.Set(context, accountService);
                        if (accountService.ExecutedActions != null)
                        this.ExecutedActionsData.Set(context, accountService.ExecutedActions.ExecutedActionsData);
                    }
                    break;
            }
        }
    }
}
