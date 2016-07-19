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
        [RequiredArgument]
        public InArgument<EntityType> EntityType { get; set; }

        [RequiredArgument]
        public InArgument<long> EntityId { get; set; }

        [RequiredArgument]
        public InArgument<Guid> StatusDefinitionId { get; set; }

        [RequiredArgument]
        public InArgument<Object> ActionProvisioningData { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            var entityType = this.EntityType.Get(context);
            switch(entityType)
            {
                case Retail.BusinessEntity.Entities.EntityType.Account:
                    AccountManager accountManager = new AccountManager();
                    var account = accountManager.UpdateStatus(this.EntityId.Get(context),this.StatusDefinitionId.Get(context));
                    break;
                case Retail.BusinessEntity.Entities.EntityType.AccountService:
                    AccountServiceManager accountServiceManager = new AccountServiceManager();
                    var accountService = accountServiceManager.UpdateStatus(this.EntityId.Get(context), this.StatusDefinitionId.Get(context));
                    break;
            }
        }
    }
}
