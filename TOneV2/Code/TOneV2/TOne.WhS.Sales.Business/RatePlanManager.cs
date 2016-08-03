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

        IRatePlanDataManager _dataManager;
        RoutingProductManager _routingProductManager;

        #endregion

        #region Constructors

        public RatePlanManager()
        {
            _dataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            _routingProductManager = new RoutingProductManager();
        }

        #endregion

        #region Public Methods

        public bool ValidateCustomer(int customerId, DateTime effectiveOn)
        {
            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            CustomerSellingProduct customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(customerId, effectiveOn, false);

            return customerSellingProduct != null;
        }

        // Remove the call to GetSellingProductId
        #region Get Default Item

        public DefaultItem GetDefaultItem(SalePriceListOwnerType ownerType, int ownerId)
        {
            DefaultItem defaultItem = new DefaultItem();

            SaleEntityZoneRoutingProductLocator locator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(DateTime.Now));
            SaleEntityZoneRoutingProduct routingProduct;

            routingProduct = (ownerType == SalePriceListOwnerType.SellingProduct) ?
                locator.GetSellingProductDefaultRoutingProduct(ownerId) :
                locator.GetCustomerDefaultRoutingProduct(ownerId, (int)GetSellingProductId(ownerType, ownerId, DateTime.Now, false));

            if (routingProduct != null)
            {
                defaultItem.CurrentRoutingProductId = routingProduct.RoutingProductId;
                defaultItem.CurrentRoutingProductName = _routingProductManager.GetRoutingProductName(routingProduct.RoutingProductId);
                defaultItem.CurrentRoutingProductBED = routingProduct.BED;
                defaultItem.CurrentRoutingProductEED = routingProduct.EED;
                defaultItem.IsCurrentRoutingProductEditable = IsDefaultRoutingProductEditable(ownerType, routingProduct.Source);
            }

            SetDefaultItemChanges(ownerType, ownerId, defaultItem);

            return defaultItem;
        }

        bool IsDefaultRoutingProductEditable(SalePriceListOwnerType ownerType, SaleEntityZoneRoutingProductSource routingProductSource)
        {
            if (ownerType == SalePriceListOwnerType.SellingProduct && routingProductSource == SaleEntityZoneRoutingProductSource.ProductDefault)
                return true;
            else if (ownerType == SalePriceListOwnerType.Customer && routingProductSource == SaleEntityZoneRoutingProductSource.CustomerDefault)
                return true;
            return false;
        }

        void SetDefaultItemChanges(SalePriceListOwnerType ownerType, int ownerId, DefaultItem defaultItem)
        {
            Changes changes = _dataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);

            if (changes != null && changes.DefaultChanges != null)
            {
                DraftNewDefaultRoutingProduct newRoutingProduct = changes.DefaultChanges.NewDefaultRoutingProduct;
                DraftChangedDefaultRoutingProduct routingProductChange = changes.DefaultChanges.DefaultRoutingProductChange;

                if (newRoutingProduct != null)
                {
                    defaultItem.NewRoutingProductId = newRoutingProduct.DefaultRoutingProductId;
                    defaultItem.NewRoutingProductName = _routingProductManager.GetRoutingProductName(newRoutingProduct.DefaultRoutingProductId);
                    defaultItem.NewRoutingProductBED = newRoutingProduct.BED;
                    defaultItem.NewRoutingProductEED = newRoutingProduct.EED;
                }
                else if (defaultItem.CurrentRoutingProductId != null && routingProductChange != null)
                    defaultItem.RoutingProductChangeEED = routingProductChange.EED;
            }
        }

        #endregion

        #region Get Zone Letters

        public IEnumerable<char> GetZoneLetters(SalePriceListOwnerType ownerType, int ownerId)
        {
            IEnumerable<char> zoneLetters = null;

            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                IEnumerable<SaleZone> saleZones = GetSellingProductZones(ownerId, DateTime.Now);

                if (saleZones != null)
                    zoneLetters = saleZones.MapRecords(itm => char.ToUpper(itm.Name[0]), itm => itm.Name != null && itm.Name.Length > 0).Distinct().OrderBy(itm => itm);
            }
            else if (ownerType == SalePriceListOwnerType.Customer)
            {
                CustomerZoneManager customerZoneManager = new CustomerZoneManager();
                zoneLetters = customerZoneManager.GetCustomerZoneLetters(ownerId);
            }

            return zoneLetters;
        }

        IEnumerable<SaleZone> GetSellingProductZones(int sellingProductId, DateTime effectiveOn)
        {
            IEnumerable<SaleZone> zones;

            SellingProductManager sellingProductManager = new SellingProductManager();
            int? sellingNumberPlanId = sellingProductManager.GetSellingNumberPlanId(sellingProductId);

            if (!sellingNumberPlanId.HasValue)
                throw new NullReferenceException("sellingNumberPlanId");

            SaleZoneManager saleZoneManager = new SaleZoneManager();
            zones = saleZoneManager.GetSaleZones(sellingNumberPlanId.Value, effectiveOn);

            return zones;
        }

        #endregion

        #region Get Zone Item(s)

        public IEnumerable<ZoneItem> GetZoneItems(ZoneItemsInput input)
        {
            List<ZoneItem> zoneItems = null;
            int? sellingNumberPlanId = GetSellingNumberPlanId(input.Filter.OwnerType, input.Filter.OwnerId);

            RatePlanZoneManager manager = new RatePlanZoneManager();
            IEnumerable<SaleZone> zones = manager.GetRatePlanZones(input.Filter.OwnerType, input.Filter.OwnerId, sellingNumberPlanId, DateTime.Now, input.Filter.ZoneLetter, input.FromRow, input.ToRow);

            if (zones != null)
            {
                zoneItems = new List<ZoneItem>();
                Changes changes = _dataManager.GetChanges(input.Filter.OwnerType, input.Filter.OwnerId, RatePlanStatus.Draft);

                int? sellingProductId = GetSellingProductId(input.Filter.OwnerType, input.Filter.OwnerId, DateTime.Now, false);

                if (sellingProductId == null)
                    throw new Exception("Selling product does not exist");

                int? customerId = null;
                if (input.Filter.OwnerType == SalePriceListOwnerType.Customer)
                    customerId = input.Filter.OwnerId;

                ZoneRateSetter rateSetter = new ZoneRateSetter(input.Filter.OwnerType, input.Filter.OwnerId, sellingProductId, DateTime.Now, changes);
                ZoneRoutingProductSetter routingProductSetter = new ZoneRoutingProductSetter((int)sellingProductId, customerId, DateTime.Now, changes);

                foreach (SaleZone zone in zones)
                {
                    ZoneItem zoneItem = new ZoneItem()
                    {
                        ZoneId = zone.SaleZoneId,
                        ZoneName = zone.Name,
                        ZoneBED = zone.BED,
                        ZoneEED = zone.EED
                    };

                    rateSetter.SetZoneRate(zoneItem);
                    routingProductSetter.SetZoneRoutingProduct(zoneItem);

                    zoneItems.Add(zoneItem);
                }

                IEnumerable<RPZone> rpZones = zoneItems.MapRecords(itm => new RPZone() { RoutingProductId = itm.EffectiveRoutingProductId, SaleZoneId = itm.ZoneId });
                ZoneRouteOptionSetter routeOptionSetter = new ZoneRouteOptionSetter(input.Filter.RoutingDatabaseId, input.Filter.PolicyConfigId, input.Filter.NumberOfOptions, rpZones, input.Filter.CostCalculationMethods, input.Filter.CostCalculationMethodConfigId, input.Filter.RateCalculationMethod);
                routeOptionSetter.SetZoneRouteOptionProperties(zoneItems);
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

            int? sellingProductId = GetSellingProductId(input.OwnerType, input.OwnerId, DateTime.Now, false);
            Changes changes = _dataManager.GetChanges(input.OwnerType, input.OwnerId, RatePlanStatus.Draft);

            ZoneRateSetter rateSetter = new ZoneRateSetter(input.OwnerType, input.OwnerId, sellingProductId, DateTime.Now, changes);
            rateSetter.SetZoneRate(zoneItem);

            if (sellingProductId == null)
                throw new Exception("Selling product does not exist");

            int? customerId = null;
            if (input.OwnerType == SalePriceListOwnerType.Customer)
                customerId = input.OwnerId;

            ZoneRoutingProductSetter routingProductSetter = new ZoneRoutingProductSetter((int)sellingProductId, customerId, DateTime.Now, changes);
            routingProductSetter.SetZoneRoutingProduct(zoneItem);

            RPZone rpZone = new RPZone() { RoutingProductId = zoneItem.EffectiveRoutingProductId, SaleZoneId = zoneItem.ZoneId };
            ZoneRouteOptionSetter routeOptionSetter = new ZoneRouteOptionSetter(input.RoutingDatabaseId, input.PolicyConfigId, input.NumberOfOptions, new List<RPZone>() { rpZone }, input.CostCalculationMethods, input.RateCalculationCostColumnConfigId, input.RateCalculationMethod);
            routeOptionSetter.SetZoneRouteOptionProperties(new List<ZoneItem>() { zoneItem });

            return zoneItem;
        }

        #endregion

        public bool SyncTempDataWithDB(long processInstanceId, int? salePriceListId, SalePriceListOwnerType ownerType, int ownerId, int currencyId, DateTime effectiveOn)
        {
            var ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            return ratePlanDataManager.SyncTempDataWithDB(processInstanceId, salePriceListId, ownerType, ownerId, currencyId, effectiveOn);
        }

        public int? GetSellingNumberPlanId(SalePriceListOwnerType ownerType, int ownerId)
        {
            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                SellingProductManager sellingProductManager = new SellingProductManager();
                return sellingProductManager.GetSellingNumberPlanId(ownerId);
            }
            else
            {
                CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
                return carrierAccountManager.GetSellingNumberPlanId(ownerId, CarrierAccountType.Customer);
            }
        }

        public RatePlanSettingsData GetRatePlanSettingsData()
        {
            var settingManager = new SettingManager();
            Setting setting = settingManager.GetSettingByType(Constants.RatePlanSettingsType);
            if (setting == null)
                throw new NullReferenceException("setting");
            if (setting.Data == null)
                throw new NullReferenceException("setting.Data");
            var ratePlanSettings = setting.Data as RatePlanSettingsData;
            if (ratePlanSettings == null)
                throw new NullReferenceException("ratePlanSettings");
            return ratePlanSettings;
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

        #endregion
    }
}
