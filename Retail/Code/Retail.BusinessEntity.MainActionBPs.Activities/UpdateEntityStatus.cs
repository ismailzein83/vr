using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Retail.BusinessEntity.Entities;

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
            throw new NotImplementedException();
        }
    }
}
