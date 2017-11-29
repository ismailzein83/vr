using System;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    #region Argument Classes

    public class FinalizeCustomerRouteDatabaseInput
    {
        public int RoutingDatabaseId { get; set; }
    }

    public class FinalizeCustomerRouteDatabaseOutput
    {

    }

    #endregion

    public sealed class FinalizeCustomerRouteDatabase : BaseAsyncActivity<FinalizeCustomerRouteDatabaseInput, FinalizeCustomerRouteDatabaseOutput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        protected override FinalizeCustomerRouteDatabaseOutput DoWorkWithResult(FinalizeCustomerRouteDatabaseInput inputArgument, AsyncActivityHandle handle)
        {
            IRoutingDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRoutingDataManager>();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            RoutingDatabase routingDatabase = routingDatabaseManager.GetRoutingDatabase(inputArgument.RoutingDatabaseId);

            dataManager.RoutingDatabase = routingDatabase;

            ConfigManager routingConfigManager = new ConfigManager();
            int commandTimeoutInSeconds = routingConfigManager.GetCustomerRouteIndexesCommandTimeoutInSeconds();
            int? maxDOP = routingConfigManager.GetCustomerRouteMaxDOP();

            dataManager.FinalizeCustomerRouteDatabase((message) =>
            {
                handle.SharedInstanceData.WriteTrackingMessage(Vanrise.Entities.LogEntryType.Information, message, null);
            }, commandTimeoutInSeconds, maxDOP);

            return new FinalizeCustomerRouteDatabaseOutput();
        }

        protected override FinalizeCustomerRouteDatabaseInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new FinalizeCustomerRouteDatabaseInput()
            {
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, FinalizeCustomerRouteDatabaseOutput result)
        {

        }
    }
}