using System;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.Routing.BP.Arguments;
using TOne.WhS.Routing.Data;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.Business
{
    public class RoutingEntitiesUpdaterBPDefinitionSettings : Vanrise.BusinessProcess.Business.DefaultBPDefinitionExtendedSettings
    {
        public const int todIntervalInMinutes = 30;

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

            RoutingEntityDetails beRouteInfoDetails = routingEntityDetailsDataManager.GetRoutingEntityDetails(RoutingEntityType.BERouteInfo);
            beRouteInfoDetails.ThrowIfNull("routingEntityDetails");
            BERouteInfo beRouteInfo = beRouteInfoDetails.RoutingEntityInfo.CastWithValidate<BERouteInfo>("beRouteInfo");

            RoutingEntityDetails partialRouteInfoDetails = routingEntityDetailsDataManager.GetRoutingEntityDetails(RoutingEntityType.PartialRouteInfo);
            partialRouteInfoDetails.ThrowIfNull("routingEntityDetails");
            PartialRouteInfo partialRouteInfo = partialRouteInfoDetails.RoutingEntityInfo.CastWithValidate<PartialRouteInfo>("partialRouteInfo");

            if (partialRouteInfo.FullRoutingDate > effectiveDate)
                return false;

            DateTime roundedEffectiveDate = RoundDate(effectiveDate);
            DateTime roundedLastProcessTime = RoundDate(beRouteInfo.LatestProcessDate);

            if (roundedLastProcessTime != roundedEffectiveDate)
            {
                ISaleRateDataManager saleRateDataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
                ISupplierRateDataManager supplierRateDataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();

                UpdateRoutingEntitiesUpdaterProcessInputArg(inputArg, true, saleRateDataManager.GetNextOpenOrCloseTime(effectiveDate), saleRateDataManager.GetMaximumTimeStamp(),
                    true, supplierRateDataManager.GetNextOpenOrCloseTime(effectiveDate), supplierRateDataManager.GetMaximumTimeStamp());
                return true;
            }

            DateTime? nextOpenOrCloseSaleRateTime;
            object maxSaleRateTimeStamp;

            DateTime? nextOpenOrCloseSupplierRateTime;
            object maxSupplierRateTimeStamp;

            bool rebuildCustomerZoneDetails = ShouldRebuildCustomerZoneDetails(beRouteInfo.SaleRateRouteInfo, effectiveDate, out nextOpenOrCloseSaleRateTime, out maxSaleRateTimeStamp);
            bool rebuildSupplierZoneDetails = ShouldRebuildSupplierZoneDetails(beRouteInfo.SupplierRateRouteInfo, effectiveDate, out nextOpenOrCloseSupplierRateTime, out maxSupplierRateTimeStamp);
            if (rebuildCustomerZoneDetails || rebuildSupplierZoneDetails)
            {
                UpdateRoutingEntitiesUpdaterProcessInputArg(inputArg, rebuildCustomerZoneDetails, nextOpenOrCloseSaleRateTime, maxSaleRateTimeStamp,
                    rebuildSupplierZoneDetails, nextOpenOrCloseSupplierRateTime, maxSupplierRateTimeStamp);
                return true;
            }

            return false;
        }

        private void UpdateRoutingEntitiesUpdaterProcessInputArg(RoutingEntitiesUpdaterProcessInput inputArg,
            bool rebuildCustomerZoneDetails, DateTime? nextOpenOrCloseSaleRateTime, Object maxSaleRateTimeStamp,
            bool rebuildSupplierZoneDetails, DateTime? nextOpenOrCloseSupplierRateTime, Object maxSupplierRateTimeStamp)
        {
            inputArg.RebuildCustomerZoneDetails = rebuildCustomerZoneDetails;
            inputArg.RebuildSupplierZoneDetails = rebuildSupplierZoneDetails;
            inputArg.NextOpenOrCloseSaleRateTime = nextOpenOrCloseSaleRateTime;
            inputArg.MaxSaleRateTimeStamp = maxSaleRateTimeStamp;
            inputArg.NextOpenOrCloseSupplierRateTime = nextOpenOrCloseSupplierRateTime;
            inputArg.MaxSupplierRateTimeStamp = maxSupplierRateTimeStamp;
        }

        private bool ShouldRebuildCustomerZoneDetails(RateRouteInfo saleRateRouteInfo, DateTime effectiveDate, out DateTime? nextOpenOrCloseSaleRateTime, out object maxSaleRateTimeStamp)
        {
            ISaleRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();

            maxSaleRateTimeStamp = saleRateRouteInfo.MaxRateTimeStamp;
            nextOpenOrCloseSaleRateTime = saleRateRouteInfo.NextOpenOrCloseRateTime;

            if (dataManager.AreSaleRatesUpdated(ref maxSaleRateTimeStamp) ||
                (saleRateRouteInfo.NextOpenOrCloseRateTime.HasValue && saleRateRouteInfo.NextOpenOrCloseRateTime <= effectiveDate))
            {
                nextOpenOrCloseSaleRateTime = dataManager.GetNextOpenOrCloseTime(effectiveDate); ;
                return true;
            }

            return false;
        }

        private bool ShouldRebuildSupplierZoneDetails(RateRouteInfo supplierRateRouteInfo, DateTime effectiveDate, out DateTime? nextOpenOrCloseSupplierRateTime, out object maxSupplierRateTimeStamp)
        {
            ISupplierRateDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierRateDataManager>();

            maxSupplierRateTimeStamp = supplierRateRouteInfo.MaxRateTimeStamp;
            nextOpenOrCloseSupplierRateTime = supplierRateRouteInfo.NextOpenOrCloseRateTime;

            if (dataManager.AreSupplierRatesUpdated(ref maxSupplierRateTimeStamp) ||
                (supplierRateRouteInfo.NextOpenOrCloseRateTime.HasValue && supplierRateRouteInfo.NextOpenOrCloseRateTime <= effectiveDate))
            {
                nextOpenOrCloseSupplierRateTime = dataManager.GetNextOpenOrCloseTime(effectiveDate); ;
                return true;
            }

            return false;
        }

        private DateTime RoundDate(DateTime dateTime)
        {
            int minuteNumber = dateTime.Hour * 60 + dateTime.Minute;
            int roundedMinuteNumber = (((int)minuteNumber / todIntervalInMinutes) * todIntervalInMinutes);
            TimeSpan ts = TimeSpan.FromMinutes(roundedMinuteNumber);
            return new DateTime(dateTime.Year, dateTime.Month, dateTime.Day, ts.Hours, ts.Minutes, 0);
        }
    }
}
