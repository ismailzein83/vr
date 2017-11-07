using System;
using System.Activities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class GetLatestRoutingDatabase : CodeActivity
    {
        [RequiredArgument]
        public OutArgument<RoutingDatabase> RoutingDatabase { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            RoutingDatabase routingDatabase = new RoutingDatabaseManager().GetLatestRoutingDatabase(RoutingProcessType.CustomerRoute, RoutingDatabaseType.Current);
            this.RoutingDatabase.Set(context, routingDatabase);
        }
    }
}