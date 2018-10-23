using System;
using System.Activities;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class UpdateBERouteInfo : CodeActivity
    {
        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        [RequiredArgument]
        public InArgument<BERouteInfo> BERouteInfo { get; set; }

        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<bool> HasModifiedCustomerZoneDetails { get; set; }

        [RequiredArgument]
        public InArgument<bool> HasModifiedSupplierZoneDetails { get; set; }

        [RequiredArgument]
        public InArgument<bool> HasModifiedRoutingQualityData { get; set; }

        [RequiredArgument]
        public InArgument<int> SaleRateVersionNumber { get; set; }

        [RequiredArgument]
        public InArgument<int> SupplierRateVersionNumber { get; set; }

        [RequiredArgument]
        public InArgument<int> QualityConfigurationVersionNumber { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> NextOpenOrCloseSaleRateTime { get; set; }

        [RequiredArgument]
        public InArgument<object> MaxSaleRateTimeStamp { get; set; }

        [RequiredArgument]
        public InArgument<DateTime?> NextOpenOrCloseSupplierRateTime { get; set; }

        [RequiredArgument]
        public InArgument<object> MaxSupplierRateTimeStamp { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            //Reading Input Arguments
            RoutingDatabase routingDatabase = this.RoutingDatabase.Get(context);
            BERouteInfo beRouteInfo = this.BERouteInfo.Get(context);
            DateTime effectiveDate = this.EffectiveDate.Get(context);

            bool hasModifiedCustomerZoneDetails = this.HasModifiedCustomerZoneDetails.Get(context);
            bool hasModifiedSupplierZoneDetails = this.HasModifiedSupplierZoneDetails.Get(context);
            bool hasModifiedQualityConfigurationData = this.HasModifiedRoutingQualityData.Get(context);
            int saleRateVersionNumber = this.SaleRateVersionNumber.Get(context);
            int supplierRateVersionNumber = this.SupplierRateVersionNumber.Get(context);
            int qualityConfigurationVersionNumber = this.QualityConfigurationVersionNumber.Get(context);

            DateTime? nextOpenOrCloseSaleRateTime = this.NextOpenOrCloseSaleRateTime.Get(context);
            object maxSaleRateTimeStamp = this.MaxSaleRateTimeStamp.Get(context);
            DateTime? nextOpenOrCloseSupplierRateTime = this.NextOpenOrCloseSupplierRateTime.Get(context);
            object maxSupplierRateTimeStamp = this.MaxSupplierRateTimeStamp.Get(context);

            //Customer
            beRouteInfo.SaleRateRouteInfo.NextOpenOrCloseRateTime = nextOpenOrCloseSaleRateTime;
            beRouteInfo.SaleRateRouteInfo.MaxRateTimeStamp = maxSaleRateTimeStamp;
            if (hasModifiedCustomerZoneDetails)
                beRouteInfo.SaleRateRouteInfo.LatestVersionNumber = saleRateVersionNumber;

            //Supplier
            beRouteInfo.SupplierRateRouteInfo.NextOpenOrCloseRateTime = nextOpenOrCloseSupplierRateTime;
            beRouteInfo.SupplierRateRouteInfo.MaxRateTimeStamp = maxSupplierRateTimeStamp;
            if (hasModifiedSupplierZoneDetails)
                beRouteInfo.SupplierRateRouteInfo.LatestVersionNumber = supplierRateVersionNumber;

            //Quality
            if (hasModifiedQualityConfigurationData)
                beRouteInfo.QualityRouteInfo.LatestVersionNumber = qualityConfigurationVersionNumber;

            beRouteInfo.LatestProcessDate = effectiveDate;
            RoutingEntityDetails routingEntityDetails = new RoutingEntityDetails() { RoutingEntityType = RoutingEntityType.BERouteInfo, RoutingEntityInfo = beRouteInfo };

            IRoutingEntityDetailsDataManager routingEntityDetailsDataManager = RoutingDataManagerFactory.GetDataManager<IRoutingEntityDetailsDataManager>();
            routingEntityDetailsDataManager.RoutingDatabase = routingDatabase;
            routingEntityDetailsDataManager.ApplyRoutingEntityDetails(routingEntityDetails);
        }
    }
}