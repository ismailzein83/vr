using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Activities;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Business;
using Vanrise.BusinessProcess;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.BP.Activities
{

    public sealed class FinalizeProductRouteDatabase : CodeActivity
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }
        public InArgument<IEnumerable<RoutingCustomerInfo>> RoutingCustomerInfos { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            IRPRouteDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRPRouteDataManager>();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            dataManager.RoutingDatabase = routingDatabaseManager.GetRoutingDatabase(this.RoutingDatabaseId.Get(context));

            ConfigManager routingConfigManager = new ConfigManager();

            bool generateCostAnalysisByCustomer = routingConfigManager.GetProductRouteBuildGenerateCostAnalysisByCustomer();
            if (generateCostAnalysisByCustomer)
                dataManager.RoutingCustomerInfo = this.RoutingCustomerInfos.Get(context);

            int commandTimeoutInSeconds = routingConfigManager.GetProductRouteIndexesCommandTimeoutInSeconds();
            int? maxDOP = routingConfigManager.GetProductRouteMaxDOP();

            dataManager.FinalizeProductRoute((message) =>
            {
                context.GetSharedInstanceData().WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, message, null);
            }, commandTimeoutInSeconds, maxDOP);
        }
    }
}

