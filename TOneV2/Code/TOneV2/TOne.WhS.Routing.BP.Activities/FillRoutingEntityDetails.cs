using System;
using System.Activities;
using Vanrise.BusinessProcess;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    #region Argument Classes

    public class FillRoutingEntityDetailsInput
    {
        public int RoutingDatabaseId { get; set; }
        public DateTime EffectiveDate { get; set; }

        public object MaxSaleRateTimeStamp { get; set; }

        public object MaxSupplierRateTimeStamp { get; set; }

        public DateTime? NextOpenOrCloseSaleRateTime { get; set; }

        public DateTime? NextOpenOrCloseSupplierRateTime { get; set; }
    }

    public class FillRoutingEntityDetailsOutput
    {

    }

    #endregion

    public sealed class FillRoutingEntityDetails : BaseAsyncActivity<FillRoutingEntityDetailsInput, FillRoutingEntityDetailsOutput>
    {
        [RequiredArgument]
        public InArgument<int> RoutingDatabaseId { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<object> MaxSaleRateTimeStamp { get; set; }

        [RequiredArgument]
        public InArgument<object> MaxSupplierRateTimeStamp { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> NextOpenOrCloseSaleRateTime { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> NextOpenOrCloseSupplierRateTime { get; set; }

        protected override FillRoutingEntityDetailsOutput DoWorkWithResult(FillRoutingEntityDetailsInput inputArgument, AsyncActivityHandle handle)
        {
            RoutingDatabaseManager routingDatabaseManager = new RoutingDatabaseManager();
            RoutingDatabase routingDatabase = routingDatabaseManager.GetRoutingDatabase(inputArgument.RoutingDatabaseId);

            if (routingDatabase.Type == RoutingDatabaseType.Current)
            {
                IRoutingEntityDetailsDataManager routingEntityDetailsDataManager = RoutingDataManagerFactory.GetDataManager<IRoutingEntityDetailsDataManager>();
                routingEntityDetailsDataManager.RoutingDatabase = routingDatabase;

                RateRouteInfo saleRateRouteInfo = new RateRouteInfo() { LatestVersionNumber = 0, MaxRateTimeStamp = inputArgument.MaxSaleRateTimeStamp, NextOpenOrCloseRateTime = inputArgument.NextOpenOrCloseSaleRateTime };
                RateRouteInfo supplierRateRouteInfo = new RateRouteInfo() { LatestVersionNumber = 0, MaxRateTimeStamp = inputArgument.MaxSupplierRateTimeStamp, NextOpenOrCloseRateTime = inputArgument.NextOpenOrCloseSupplierRateTime };

                PartialRouteInfo partialRouteInfo = new PartialRouteInfo() { LastVersionNumber = 0, LatestRoutingDate = inputArgument.EffectiveDate, SaleRateRouteInfo = saleRateRouteInfo, SupplierRateRouteInfo = supplierRateRouteInfo };
                RoutingEntityDetails partialRoutingEntityDetails = new RoutingEntityDetails() { RoutingEntityType = RoutingEntityType.PartialRouteInfo, RoutingEntityInfo = partialRouteInfo };
                routingEntityDetailsDataManager.ApplyRoutingEntityDetails(partialRoutingEntityDetails);

                BERouteInfo beRouteInfo = new BERouteInfo() { LatestCostRateVersionNumber = 0, LatestSaleRateVersionNumber = 0 };
                RoutingEntityDetails beRoutingEntityDetails = new RoutingEntityDetails() { RoutingEntityType = RoutingEntityType.BERoute, RoutingEntityInfo = beRouteInfo };
                routingEntityDetailsDataManager.ApplyRoutingEntityDetails(beRoutingEntityDetails);
            }

            return new FillRoutingEntityDetailsOutput();
        }

        protected override FillRoutingEntityDetailsInput GetInputArgument(AsyncCodeActivityContext context)
        {
            return new FillRoutingEntityDetailsInput()
            {
                RoutingDatabaseId = this.RoutingDatabaseId.Get(context),
                EffectiveDate = this.EffectiveDate.Get(context),
                MaxSaleRateTimeStamp = this.MaxSaleRateTimeStamp.Get(context),
                MaxSupplierRateTimeStamp = this.MaxSupplierRateTimeStamp.Get(context),
                NextOpenOrCloseSaleRateTime = this.NextOpenOrCloseSaleRateTime.Get(context),
                NextOpenOrCloseSupplierRateTime = this.NextOpenOrCloseSupplierRateTime.Get(context)
            };
        }

        protected override void OnWorkComplete(AsyncCodeActivityContext context, FillRoutingEntityDetailsOutput result)
        {

        }
    }
}