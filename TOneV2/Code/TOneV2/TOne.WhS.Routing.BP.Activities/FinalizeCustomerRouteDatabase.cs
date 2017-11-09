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
        public DateTime EffectiveDate { get; set; }
    }

    public class FinalizeCustomerRouteDatabaseOutput
    {

    }

    #endregion

    public sealed class FinalizeCustomerRouteDatabase : BaseAsyncActivity<FinalizeCustomerRouteDatabaseInput, FinalizeCustomerRouteDatabaseOutput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        protected override FinalizeCustomerRouteDatabaseOutput DoWorkWithResult(FinalizeCustomerRouteDatabaseInput inputArgument, AsyncActivityHandle handle)
        {
            IRoutingDataManager dataManager = RoutingDataManagerFactory.GetDataManager<IRoutingDataManager>();
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            RoutingDatabase routingDatabase = routingDatabaseManager.GetRoutingDatabase(inputArgument.RoutingDatabaseId);

            dataManager.RoutingDatabase = routingDatabase;

            ConfigManager routingConfigManager = new ConfigManager();
            int commandTimeoutInSeconds = routingConfigManager.GetCustomerRouteIndexesCommandTimeoutInSeconds();
            int? maxDOP = routingConfigManager.GetCustomerRouteMaxDOP();

            if (routingDatabase.Type == RoutingDatabaseType.Current)
            {
                PartialRouteInfo partialRouteInfo = new PartialRouteInfo() { LastVersionNumber = 0, LatestRoutingDate = inputArgument.EffectiveDate };
                IPartialRouteInfoDataManager partialRouteInfoDataManager = RoutingDataManagerFactory.GetDataManager<IPartialRouteInfoDataManager>();
                partialRouteInfoDataManager.RoutingDatabase = routingDatabase;
                partialRouteInfoDataManager.ApplyPartialRouteInfo(partialRouteInfo);
            }

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
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context),
                EffectiveDate = this.EffectiveDate.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, FinalizeCustomerRouteDatabaseOutput result)
        {

        }
    }
}