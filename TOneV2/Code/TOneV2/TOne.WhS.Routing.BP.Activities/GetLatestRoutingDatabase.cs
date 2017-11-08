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
        public InArgument<RoutingProcessType> RoutingProcessType { get; set; }

        [RequiredArgument]
        public InArgument<RoutingDatabaseType> RoutingDatabaseType { get; set; }

        [RequiredArgument]
        public OutArgument<RoutingDatabase> RoutingDatabase { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            RoutingDatabase routingDatabase = new RoutingDatabaseManager().GetLatestRoutingDatabase(this.RoutingProcessType.Get(context), this.RoutingDatabaseType.Get(context));
            this.RoutingDatabase.Set(context, routingDatabase);
        }
    }
}