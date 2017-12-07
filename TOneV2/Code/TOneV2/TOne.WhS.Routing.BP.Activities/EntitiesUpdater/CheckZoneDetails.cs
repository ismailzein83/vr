using System;
using System.Activities;
using Vanrise.Common;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Data;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class CheckZoneDetails : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<RoutingDatabase> RoutingDatabase { get; set; }

        [RequiredArgument]
        public InArgument<BERouteInfo> BERouteInfo { get; set; }

        [RequiredArgument]
        public InArgument<PartialRouteInfo> PartialRouteInfo { get; set; }

        [RequiredArgument]
        public OutArgument<bool> RebuildCustomerZoneDetails { get; set; }

        [RequiredArgument]
        public OutArgument<bool> RebuildSupplierZoneDetails { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            this.RebuildCustomerZoneDetails.Set(context, true);
            this.RebuildSupplierZoneDetails.Set(context, true);
            //if (routingEntitiesUpdaterProcessState == null || !routingEntitiesUpdaterProcessState.LastProcessTime.HasValue)
            //{
            //    this.RebuildCustomerZoneDetails.Set(context, true);
            //    this.RebuildSupplierZoneDetails.Set(context, true);
            //    return;
            //}

            //DateTime lastProcessTime = routingEntitiesUpdaterProcessState.LastProcessTime.Value;
            //DateTime modifiedLastProcessTime = ModifyDate(lastProcessTime);

            //DateTime effectiveDate = this.EffectiveDate.Get(context);
            //DateTime modifiedEffectiveDate = ModifyDate(effectiveDate);

            //if (modifiedLastProcessTime != modifiedEffectiveDate)
            //{
            //    this.RebuildCustomerZoneDetails.Set(context, true);
            //    this.RebuildSupplierZoneDetails.Set(context, true);
            //    return;
            //}

            //BERouteInfo beRouteInfo = this.BERouteInfo.Get(context);
            //beRouteInfo.ThrowIfNull("beRouteInfo");
            //beRouteInfo.SaleRateRouteInfo.ThrowIfNull("beRouteInfo.SaleRateRouteInfo");
            //beRouteInfo.SupplierRateRouteInfo.ThrowIfNull("beRouteInfo.SupplierRateRouteInfo");
            
            //PartialRouteInfo partialRouteInfo = this.PartialRouteInfo.Get(context);
            //partialRouteInfo.ThrowIfNull("partialRouteInfo");

            //if (partialRouteInfo.FullRoutingDate > effectiveDate)
            //{
            //    this.RebuildCustomerZoneDetails.Set(context, false);
            //    this.RebuildSupplierZoneDetails.Set(context, false);
            //    return;
            //}

            //bool rebuildCustomerZoneDetails = ShouldRebuildCustomerZoneDetails(beRouteInfo.SaleRateRouteInfo, effectiveDate);
            //bool rebuildSupplierZoneDetails = ShouldRebuildSupplierZoneDetails(beRouteInfo.SupplierRateRouteInfo, effectiveDate);

            //this.RebuildCustomerZoneDetails.Set(context, rebuildCustomerZoneDetails);
            //this.RebuildSupplierZoneDetails.Set(context, rebuildSupplierZoneDetails);
        }

        //private DateTime ModifyDate(DateTime dateTime)
        //{
        //    int minuteNumber = dateTime.Hour * 60 + dateTime.Minute;
        //    int roundedMinuteNumber = (((int)minuteNumber / 30) * 30);
        //    TimeSpan ts = TimeSpan.FromMinutes(roundedMinuteNumber);
        //    return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, ts.Hours, ts.Minutes, 0);
        //}

        //private bool ShouldRebuildCustomerZoneDetails(RateRouteInfo saleRateRouteInfo, DateTime effectiveDate)
        //{
        //    if (saleRateRouteInfo.NextOpenOrCloseRateTime.HasValue && saleRateRouteInfo.NextOpenOrCloseRateTime <= effectiveDate)
        //        return true;

        //    ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();

        //    object maxSaleRateTimeStamp = saleRateRouteInfo.MaxRateTimeStamp;
        //    if (dataManager.AreSaleRatesUpdated(ref maxSaleRateTimeStamp))
        //    {
        //        saleRateRouteInfo.MaxRateTimeStamp = maxSaleRateTimeStamp;
        //        return true;
        //    }
        //    return false;
        //}

        //private bool ShouldRebuildSupplierZoneDetails(RateRouteInfo supplierRateRouteInfo, DateTime effectiveDate)
        //{
        //    if (supplierRateRouteInfo.NextOpenOrCloseRateTime.HasValue && supplierRateRouteInfo.NextOpenOrCloseRateTime <= effectiveDate)
        //        return true;

        //    ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();

        //    object maxSupplierRateTimeStamp = supplierRateRouteInfo.MaxRateTimeStamp;
        //    if (dataManager.AreSupplierRatesUpdated(ref maxSupplierRateTimeStamp))
        //    {
        //        supplierRateRouteInfo.MaxRateTimeStamp = maxSupplierRateTimeStamp;
        //        return true;
        //    }

        //    return false;
        //}
    }
}