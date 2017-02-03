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
        public InArgument<Guid> AccountBEDefinitionId { get; set; }
        public InArgument<long> AccountId { get; set; }
        public OutArgument<dynamic> Entity { get; set; }
      //  public OutArgument<List<Object>> ExecutedActionsData { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            AccountBEManager accountBEManager = new AccountBEManager();
            var account = accountBEManager.GetAccount(this.AccountBEDefinitionId.Get(context), this.AccountId.Get(context));
            this.Entity.Set(context, account);
            //if (account.ExecutedActions != null)
            //    this.ExecutedActionsData.Set(context, account.ExecutedActions.ExecutedActionsData);
        }
    }
}
