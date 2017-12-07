using System;
using Vanrise.Common;
using TOne.WhS.Routing.BP.Arguments;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Routing.Business
{
    public class RoutingEntitiesUpdaterBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public override bool CanRunBPInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionCanRunBPInstanceContext context)
        {
            context.IntanceToRun.ThrowIfNull("context.IntanceToRun");
            RoutingEntitiesUpdaterProcessInput routingEntitiesUpdaterProcessInputArg = context.IntanceToRun.InputArgument.CastWithValidate<RoutingEntitiesUpdaterProcessInput>("context.IntanceToRun.InputArgument");

            foreach (var startedBPInstance in context.GetStartedBPInstances())
            {
                RoutingEntitiesUpdaterProcessInput startedBPRoutingEntitiesUpdaterArg = startedBPInstance.InputArgument as RoutingEntitiesUpdaterProcessInput;
                if (startedBPRoutingEntitiesUpdaterArg != null)
                {
                    context.Reason = "Another Routing Entities Updater instance is running";
                    return false;
                }

                RoutingProcessInput startedBPInstanceRoutingArg = startedBPInstance.InputArgument as RoutingProcessInput;
                if (startedBPInstanceRoutingArg != null && startedBPInstanceRoutingArg.RoutingDatabaseType == Entities.RoutingDatabaseType.Current)
                {
                    context.Reason = "Route Build instance of type Current is running";
                    return false;
                }
            }

            return true;
        }

        public override bool ShouldCreateScheduledInstance(Vanrise.BusinessProcess.Entities.IBPDefinitionShouldCreateScheduledInstanceContext context)
        {
            DateTime effectiveDate = DateTime.Now;

            RoutingDatabase routingDatabase = new RoutingDatabaseManager().GetLatestRoutingDatabase(RoutingProcessType.CustomerRoute, RoutingDatabaseType.Current);
            if (routingDatabase == null)
                return false;

            RoutingEntitiesUpdaterProcessInput inputArg = context.BaseProcessInputArgument.CastWithValidate<RoutingEntitiesUpdaterProcessInput>("context.BaseProcessInputArgument");

            IRoutingEntityDetailsDataManager routingEntityDetailsDataManager = RoutingDataManagerFactory.GetDataManager<IRoutingEntityDetailsDataManager>();
            routingEntityDetailsDataManager.RoutingDatabase = routingDatabase;

            RoutingEntityDetails routingEntityDetails = routingEntityDetailsDataManager.GetRoutingEntityDetails(RoutingEntityType.BERouteInfo);
            routingEntityDetails.ThrowIfNull("routingEntityDetails");

            BERouteInfo beRouteInfo = routingEntityDetails.RoutingEntityInfo.CastWithValidate<BERouteInfo>("beRouteInfo");
            PartialRouteInfo partialRouteInfo = routingEntityDetails.RoutingEntityInfo.CastWithValidate<PartialRouteInfo>("partialRouteInfo");

            return false;





            //RoutingDatabase routingDatabase = new RoutingDatabaseManager().GetLatestRoutingDatabase(RoutingProcessType.CustomerRoute, RoutingDatabaseType.Current);
            //if (routingDatabase == null)
            //    return false;

            //RoutingEntitiesUpdaterProcessInput inputArg = context.BaseProcessInputArgument.CastWithValidate<RoutingEntitiesUpdaterProcessInput>("context.BaseProcessInputArgument");

            //IRoutingEntityDetailsDataManager routingEntityDetailsDataManager = RoutingDataManagerFactory.GetDataManager<IRoutingEntityDetailsDataManager>();
            //routingEntityDetailsDataManager.RoutingDatabase = routingDatabase;
            //RoutingEntityDetails routingEntityDetails = routingEntityDetailsDataManager.GetRoutingEntityDetails(RoutingEntityType.BERouteInfo);

            //DateTime now = DateTime.Now;
            //if (routingEntityDetails != null)
            //{
            //    PartialRouteInfo partialRouteInfo = routingEntityDetails.RoutingEntityInfo.CastWithValidate<PartialRouteInfo>("routingEntityDetails.RoutingEntityInfo");
            //    if (partialRouteInfo.NextOpenOrCloseRuleTime.HasValue && partialRouteInfo.NextOpenOrCloseRuleTime < now)
            //        return true;
            //}

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

            //return false;
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
