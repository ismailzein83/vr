using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.LCR.Data;
using TOne.LCR.Entities;

namespace TOne.LCRProcess.Activities
{

    public sealed class GetCurrentDatabaseId : CodeActivity
    {
        [RequiredArgument]
        public InArgument<RoutingDatabaseType> RoutingDatabaseType { get; set; }

        [RequiredArgument]
        public OutArgument<int> RoutingDatabaseId { get; set; }
        protected override void Execute(CodeActivityContext context)
        {
            IRoutingDataManager routingDatabaseManager = LCRDataManagerFactory.GetDataManager<IRoutingDataManager>();
            routingDatabaseManager.RoutingDatabaseType = this.RoutingDatabaseType.Get(context);
            int databaseId = routingDatabaseManager.DatabaseId;
            this.RoutingDatabaseId.Set(context, databaseId);
        }
    }
}
