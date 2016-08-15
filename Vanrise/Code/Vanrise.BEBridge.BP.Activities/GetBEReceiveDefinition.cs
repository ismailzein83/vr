using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using Vanrise.BEBridge.Entities;
using Vanrise.BEBridge.Business;

namespace Vanrise.BEBridge.BP.Activities
{
    public sealed class GetBEReceiveDefinition : CodeActivity
    {
        [RequiredArgument]
        public InArgument<Guid> BEReceiveDefinitionId { get; set; }
        [RequiredArgument]
        public OutArgument<BEReceiveDefinition> BEReceiveDefinition { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            BEReceiveDefinitionManager manager = new BEReceiveDefinitionManager();
            BEReceiveDefinition.Set(context, manager.GetBEReceiveDefinition(BEReceiveDefinitionId.Get(context)));
        }
    }
}
