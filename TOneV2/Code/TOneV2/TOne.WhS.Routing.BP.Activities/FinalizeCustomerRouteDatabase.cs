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

    public sealed class FinalizeCustomerRouteDatabase : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IRoutingDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRoutingDataManager>();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            dataManager.RoutingDatabase = routingDatabaseManager.GetRoutingDatabase(this.RoutingDatabaseId.Get(context));

            dataManager.FinalizeCustomerRouteDatabase((message) => 
            {
                context.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, message, null);
            });
        }
    }
}

