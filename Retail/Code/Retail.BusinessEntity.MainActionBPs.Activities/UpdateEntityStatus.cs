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

        protected override void Execute(CodeActivityContext context)
        {
            var accountBEDefinitionId = this.AccountBEDefinitionId.Get(context);
            var accountId = this.AccountId.Get(context);
            var statusDefinitionId =  this.StatusDefinitionId.Get(context);
            AccountBEManager accountBEManager = new AccountBEManager();
            accountBEManager.UpdateStatus(accountBEDefinitionId, accountId, statusDefinitionId);
        }
    }
}
