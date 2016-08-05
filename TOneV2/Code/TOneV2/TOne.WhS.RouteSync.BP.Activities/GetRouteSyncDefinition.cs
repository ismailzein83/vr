using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Business;

namespace TOne.WhS.RouteSync.BP.Activities
{

    public sealed class GetRouteSyncDefinition : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> RouteSyncDefinitionId { get; set; }

        [RequiredArgument]
        public OutArgument<RouteSyncDefinition> RouteSyncDefinition { get; set; }
        
        protected override void Execute(CodeActivityContext context)
        {
            RouteSyncDefinitionManager routeSyncDefinitionManager = new RouteSyncDefinitionManager();
            RouteSyncDefinition routeSyncDefinition = routeSyncDefinitionManager.GetRouteSyncDefinitionById(RouteSyncDefinitionId.Get(context));
            RouteSyncDefinition.Set(context, routeSyncDefinition);
        }
    }
}
