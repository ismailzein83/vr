using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Business;
using Vanrise.BusinessProcess;

namespace TOne.WhS.Routing.BP.Activities
{

    public sealed class FinalizeProductRouteDatabase : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IRPRouteDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            dataManager.RoutingDatabase = routingDatabaseManager.GetRoutingDatabase(this.RoutingDatabaseId.Get(context));

            dataManager.FinalizeProductRoute((message) => 
            {
                context.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, message, null);
            });
        }
    }
}

