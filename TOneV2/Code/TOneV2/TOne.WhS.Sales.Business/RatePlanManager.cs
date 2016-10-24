using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanManager
    {
        #region Fields

        IRatePlanDataManager _dataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
        RoutingProductManager _routingProductManager = new RoutingProductManager();

        #endregion

        #region Public Methods

        public bool ValidateCustomer(int customerId, DateTime effectiveOn)
        {
            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            CustomerSellingProduct customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(customerId, effectiveOn, false);

            return customerSellingProduct != null;
        }

        #region Get Zone Letters

        public IEnumerable<char> GetZoneLetters(ZoneLettersInput input)
        {
            int sellingNumberPlanId = GetSellingNumberPlanId(input.OwnerType, input.OwnerId);
            IEnumerable<SaleZone> zones = new SaleZoneManager().GetSaleZonesByOwner(input.OwnerType, input.OwnerId, sellingNumberPlanId, input.EffectiveOn, true);
            
            zones = zones.FindAllRecords
            (
                x => (input.CountryIds == null || input.CountryIds.Contains(x.CountryId))
                && (!input.ZoneNameFilterType.HasValue || Vanrise.Common.Utilities.IsTextMatched(x.Name, input.ZoneNameFilter, input.ZoneNameFilterType.Value))
            );
            
            if (zones != null)
                return zones.MapRecords(x => char.ToUpper(x.Name[0]), x => x.Name != null && x.Name.Length > 0).Distinct().OrderBy(x => x);
            return null;
        }

        #endregion

        #region Get Zone Item(s)

        // TODO: Divide GetZoneItem(s) into GetSellingProductZoneItem(s) and GetCustomerZoneItem(s) to get rid of unnecessary sellingProductId == null and ownerType checks
        public IEnumerable<ZoneItem> GetZoneItems(ZoneItemsInput input)
        {
            List<ZoneItem> zoneItems = null;
            int sellingNumberPlanId = GetSellingNumberPlanId(input.Filter.OwnerType, input.Filter.OwnerId);

            RatePlanZoneManager manager = new RatePlanZoneManager();
            IEnumerable<SaleZone> zones = manager.GetRatePlanZones(input.Filter.OwnerType, input.Filter.OwnerId, sellingNumberPlanId, DateTime.Now, input.Filter.CountryIds, input.Filter.ZoneLetter, input.Filter.ZoneNameFilterType, input.Filter.ZoneNameFilter, input.FromRow, input.ToRow);

            if (zones != null)
            {
                zoneItems = new List<ZoneItem>();
                Changes draft = _dataManager.GetChanges(input.Filter.OwnerType, input.Filter.OwnerId, RatePlanStatus.Draft);

                int? sellingProductId = GetSellingProductId(input.Filter.OwnerType, input.Filter.OwnerId, DateTime.Now, false);

                if (sellingProductId == null)
                    throw new Exception("Selling product does not exist");

                int? customerId = null;
                if (input.Filter.OwnerType == SalePriceListOwnerType.Customer)
                    customerId = input.Filter.OwnerId;

                // TODO: Add EffectiveOn to input.Filter
                var effectiveOn = DateTime.Now.Date;

                var rateManager = new ZoneRateManager(input.Filter.OwnerType, input.Filter.OwnerId, sellingProductId, effectiveOn, draft, input.CurrencyId);
                var rpManager = new ZoneRPManager(input.Filter.OwnerType, input.Filter.OwnerId, effectiveOn, draft);
                var serviceManager = new ZoneServiceManager(input.Filter.OwnerType, input.Filter.OwnerId, effectiveOn, draft);

                foreach (SaleZone zone in zones)
                {
                    ZoneItem zoneItem = new ZoneItem()
                    {
                        ZoneId = zone.SaleZoneId,
                        ZoneName = zone.Name,
                        ZoneBED = zone.BED,
                        ZoneEED = zone.EED
                    };

                    zoneItem.IsFutureZone = (zoneItem.ZoneBED.Date > DateTime.Now.Date);

                    ZoneChanges zoneDraft = null;
                    if (draft != null && draft.ZoneChanges != null)
                        zoneDraft = draft.ZoneChanges.FindRecord(x => x.ZoneId == zone.SaleZoneId);

                    rateManager.SetZoneRate(zoneItem);

                    // TODO: Refactor the rateSetter to handle products and customers separately
                    if (input.Filter.OwnerType == SalePriceListOwnerType.SellingProduct)
                    {
                        rpManager.SetSellingProductZoneRP(zoneItem, input.Filter.OwnerId, zoneDraft);
                        serviceManager.SetSellingProductZoneService(zoneItem, input.Filter.OwnerId, zoneDraft);
                    }
                    else
                    {
                        rpManager.SetCustomerZoneRP(zoneItem, input.Filter.OwnerId, sellingProductId.Value, zoneDraft);
                        serviceManager.SetCustomerZoneService(zoneItem, input.Filter.OwnerId, sellingProductId.Value, zoneDraft);
                    }

                    zoneItems.Add(zoneItem);
                }

                IEnumerable<RPZone> rpZones = zoneItems.MapRecords(x => new RPZone() { RoutingProductId = x.EffectiveRoutingProductId, SaleZoneId = x.ZoneId });
                var routeOptionManager = new ZoneRouteOptionManager(input.Filter.OwnerType, input.Filter.OwnerId, input.Filter.RoutingDatabaseId, input.Filter.PolicyConfigId, input.Filter.NumberOfOptions, rpZones, input.Filter.CostCalculationMethods, input.Filter.CostCalculationMethodConfigId, input.Filter.RateCalculationMethod, input.CurrencyId);
                routeOptionManager.SetZoneRouteOptionProperties(zoneItems);
            }

            return zoneItems;
        }

        public ZoneItem GetZoneItem(ZoneItemInput input)
        {
            SaleZoneManager saleZoneManager = new SaleZoneManager();

            ZoneItem zoneItem = new ZoneItem()
            {
                ZoneId = input.ZoneId,
                ZoneName = saleZoneManager.GetSaleZoneName(input.ZoneId)
            };

            // TODO: Add EffectiveOn to input
            var effectiveOn = DateTime.Now.Date;

            int? sellingProductId = GetSellingProductId(input.OwnerType, input.OwnerId, effectiveOn, false);
            Changes draft = _dataManager.GetChanges(input.OwnerType, input.OwnerId, RatePlanStatus.Draft);

            ZoneRateManager rateSetter = new ZoneRateManager(input.OwnerType, input.OwnerId, sellingProductId, effectiveOn, draft, input.CurrencyId);
            rateSetter.SetZoneRate(zoneItem);

            if (sellingProductId == null)
                throw new Exception("Selling product does not exist");

            int? customerId = null;
            if (input.OwnerType == SalePriceListOwnerType.Customer)
                customerId = input.OwnerId;
            
            var rpManager = new ZoneRPManager(input.OwnerType, input.OwnerId, effectiveOn, draft);
            
            ZoneChanges zoneDraft = null;
            if (draft != null && draft.ZoneChanges != null)
                zoneDraft = draft.ZoneChanges.FindRecord(x => x.ZoneId == input.ZoneId);

            if (input.OwnerType == SalePriceListOwnerType.SellingProduct)
                rpManager.SetSellingProductZoneRP(zoneItem, input.OwnerId, zoneDraft);
            else
                rpManager.SetCustomerZoneRP(zoneItem, input.OwnerId, sellingProductId.Value, zoneDraft);

            RPZone rpZone = new RPZone() { RoutingProductId = zoneItem.EffectiveRoutingProductId, SaleZoneId = zoneItem.ZoneId };
            ZoneRouteOptionManager routeOptionSetter = new ZoneRouteOptionManager(input.OwnerType, input.OwnerId, input.RoutingDatabaseId, input.PolicyConfigId, input.NumberOfOptions, new List<RPZone>() { rpZone }, input.CostCalculationMethods, input.RateCalculationCostColumnConfigId, input.RateCalculationMethod, input.CurrencyId);
            routeOptionSetter.SetZoneRouteOptionProperties(new List<ZoneItem>() { zoneItem });

            return zoneItem;
        }
          
        #endregion

        public bool SyncImportedDataWithDB(long processInstanceId, int? salePriceListId, SalePriceListOwnerType ownerType, int ownerId, int currencyId, DateTime effectiveOn)
        {
            var ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            return ratePlanDataManager.SyncImportedDataWithDB(processInstanceId, salePriceListId, ownerType, ownerId, currencyId, effectiveOn);
        }

        public int GetSellingNumberPlanId(SalePriceListOwnerType ownerType, int ownerId)
        {
            int? sellingNumberPlanId = (ownerType == SalePriceListOwnerType.SellingProduct) ?
                new SellingProductManager().GetSellingNumberPlanId(ownerId) :
                new CarrierAccountManager().GetSellingNumberPlanId(ownerId, CarrierAccountType.Customer);
            if (!sellingNumberPlanId.HasValue)
                throw new NullReferenceException("sellingNumberPlanId");
            return sellingNumberPlanId.Value;
        }

        public RatePlanSettingsData GetRatePlanSettingsData()
        {
            object ratePlanSettingData = GetSettingData(Sales.Business.Constants.RatePlanSettingsType);
            var ratePlanSettings = ratePlanSettingData as RatePlanSettingsData;
            if (ratePlanSettings == null)
                throw new NullReferenceException("ratePlanSettings");
            return ratePlanSettings;
        }

        public SaleAreaSettingsData GetSaleAreaSettingsData()
        {
            object saleAreaSettingData = GetSettingData(BusinessEntity.Business.Constants.SaleAreaSettings);
            var saleAreaSettings = saleAreaSettingData as SaleAreaSettingsData;
            if (saleAreaSettings == null)
                throw new NullReferenceException("saleAreaSettings");
            return saleAreaSettings;
        }

        private object GetSettingData(string settingType)
        {
            var settingManager = new SettingManager();
            Setting setting = settingManager.GetSettingByType(settingType);
            if (setting == null)
                throw new NullReferenceException("setting");
            if (setting.Data == null)
                throw new NullReferenceException("setting.Data");
            return setting.Data;
        }

        public int? GetSellingProductId(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn, bool isEffectiveInFuture)
        {
            if (ownerType == SalePriceListOwnerType.Customer)
            {
                CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
                return customerSellingProductManager.GetEffectiveSellingProductId(ownerId, effectiveOn, isEffectiveInFuture); // Review what isEffectiveFuture does
            }
            else // If the owner is a selling product
                return ownerId;
        }

        public int GetSellingProductId(int customerId, DateTime effectiveOn, bool isEffectiveInFuture)
        {
            var customerSellingProductManager = new CustomerSellingProductManager();
            int? sellingProductId = customerSellingProductManager.GetEffectiveSellingProductId(customerId, effectiveOn, isEffectiveInFuture);
            if (!sellingProductId.HasValue)
                throw new NullReferenceException(String.Format("Customer '{0}' is not assigned to a SellingProduct", customerId));
            return sellingProductId.Value;
        }

        public SaleEntityZoneRate GetRate(SalePriceListOwnerType ownerType, int ownerId, long zoneId, DateTime effectiveOn)
        {
            var rateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(effectiveOn));

            if (ownerType == SalePriceListOwnerType.SellingProduct)
                return rateLocator.GetSellingProductZoneRate(ownerId, zoneId);
            else
            {
                int? sellingProductId = GetSellingProductId(ownerType, ownerId, effectiveOn, false);
                if (sellingProductId == null)
                    throw new NullReferenceException("sellingProductId");
                return rateLocator.GetCustomerZoneRate(ownerId, sellingProductId.Value, zoneId);
            }
        }

        #endregion
    }
}
