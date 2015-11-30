using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanManager
    {
        private IRatePlanDataManager _dataManager;

        public RatePlanManager()
        {
            _dataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
        }

        #region Get Zone Letters

        public IEnumerable<char> GetZoneLetters(SalePriceListOwnerType ownerType, int ownerId)
        {
            IEnumerable<char> zoneLetters = null;

            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                IEnumerable<SaleZone> saleZones = GetSellingProductZones(ownerId, DateTime.Now);

                if (saleZones != null)
                    zoneLetters = saleZones.MapRecords(zone => zone.Name[0], zone => zone.Name != null && zone.Name.Length > 0).Distinct().OrderBy(letter => letter);
            }
            else if (ownerType == SalePriceListOwnerType.Customer)
            {
                CustomerZoneManager customerZoneManager = new CustomerZoneManager();
                zoneLetters = customerZoneManager.GetCustomerZoneLetters(ownerId);
            }

            return zoneLetters;
        }
        
        #endregion

        #region Get Zone Items

        public IEnumerable<ZoneItem> GetZoneItems(ZoneItemInput input)
        {
            List<ZoneItem> zoneItems = null;
            IEnumerable<SaleZone> zones = GetZones(input.Filter.OwnerType, input.Filter.OwnerId);

            if (zones != null)
            {
                zones = GetFilteredZones(zones, input.Filter);

                if (zones != null)
                {
                    zones = GetPagedZones(zones, input.FromRow, input.ToRow);

                    if (zones != null)
                    {
                        zoneItems = new List<ZoneItem>();

                        foreach (SaleZone zone in zones)
                        {
                            ZoneItem zoneItem = new ZoneItem();

                            zoneItem.ZoneId = zone.SaleZoneId;
                            zoneItem.ZoneName = zone.Name;

                            SetZoneRate(input.Filter.OwnerType, input.Filter.OwnerId, zoneItem);
                            SetZoneRoutingProduct(input.Filter.OwnerType, input.Filter.OwnerId, zoneItem);

                            zoneItems.Add(zoneItem);
                        }

                        SetZoneItemChanges(input.Filter.OwnerType, input.Filter.OwnerId, zoneItems);
                        
                        foreach (ZoneItem zoneItem in zoneItems)
                            SetZoneEffectiveRoutingProduct(input.Filter.OwnerType, input.Filter.OwnerId, zoneItem);

                        SetRouteOptionsForZoneItems(zoneItems);
                    }
                }
            }

            return zoneItems;
        }

        private IEnumerable<SaleZone> GetZones(SalePriceListOwnerType ownerType, int ownerId)
        {
            IEnumerable<SaleZone> saleZones = null;

            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                saleZones = GetSellingProductZones(ownerId, DateTime.Now);
            }
            else if (ownerType == SalePriceListOwnerType.Customer)
            {
                CustomerZoneManager manager = new CustomerZoneManager();
                saleZones = manager.GetCustomerSaleZones(ownerId, DateTime.Now, false);
            }

            return saleZones;
        }

        private IEnumerable<SaleZone> GetSellingProductZones(int sellingProductId, DateTime effectiveOn)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            int sellingNumberPlanId = carrierAccountManager.GetSellingNumberPlanId(sellingProductId, CarrierAccountType.Customer);

            SaleZoneManager saleZoneManager = new SaleZoneManager();
            return saleZoneManager.GetSaleZones(sellingNumberPlanId, effectiveOn);
        }

        private IEnumerable<SaleZone> GetFilteredZones(IEnumerable<SaleZone> saleZones, ZoneItemFilter filter)
        {
            return saleZones.FindAllRecords(z => z.Name != null && z.Name.Length > 0 && char.ToLower(z.Name.ElementAt(0)) == char.ToLower(filter.ZoneLetter));
        }

        private IEnumerable<SaleZone> GetPagedZones(IEnumerable<SaleZone> saleZones, int fromRow, int toRow)
        {
            List<SaleZone> pagedSaleZones = null;

            if (saleZones.Count() >= fromRow)
            {
                pagedSaleZones = new List<SaleZone>();

                for (int i = fromRow - 1; i < toRow && i < saleZones.Count(); i++)
                {
                    pagedSaleZones.Add(saleZones.ElementAt(i));
                }
            }

            return pagedSaleZones;
        }

        private void SetZoneRate(SalePriceListOwnerType ownerType, int ownerId, ZoneItem zoneItem)
        {
            SaleEntityZoneRateLocator locator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(DateTime.Now));
            
            SaleEntityZoneRate zoneRate = (ownerType == SalePriceListOwnerType.SellingProduct) ?
                locator.GetSellingProductZoneRate(ownerId, zoneItem.ZoneId) :
                locator.GetCustomerZoneRate(ownerId, GetSellingProductId(ownerId), zoneItem.ZoneId);

            if (zoneRate != null)
            {
                zoneItem.IsCurrentRateEditable = (zoneRate.Source == ownerType);
                zoneItem.CurrentRateId = zoneRate.Rate.SaleRateId;
                zoneItem.CurrentRate = zoneRate.Rate.NormalRate;
                zoneItem.CurrentRateBED = zoneRate.Rate.BeginEffectiveDate;
                zoneItem.CurrentRateEED = zoneRate.Rate.EndEffectiveDate;
            }
        }

        private void SetZoneRoutingProduct(SalePriceListOwnerType ownerType, int ownerId, ZoneItem zoneItem)
        {
            SaleEntityZoneRoutingProductLocator locator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(DateTime.Now));

            SaleEntityZoneRoutingProduct zoneRoutingProduct = (ownerType == SalePriceListOwnerType.SellingProduct) ?
                locator.GetSellingProductZoneRoutingProduct(ownerId, zoneItem.ZoneId) :
                locator.GetCustomerZoneRoutingProduct(ownerId, GetSellingProductId(ownerId), zoneItem.ZoneId);

            if (zoneRoutingProduct != null)
            {
                zoneItem.CurrentRoutingProductId = zoneRoutingProduct.RoutingProductId;
                zoneItem.CurrentRoutingProductName = (zoneItem.CurrentRoutingProductId != null) ?
                    new RoutingProductManager().GetRoutingProduct((int)zoneItem.CurrentRoutingProductId).Name : null;
                zoneItem.CurrentRoutingProductBED = zoneRoutingProduct.BED;
                zoneItem.CurrentRoutingProductEED = zoneRoutingProduct.EED;

                zoneItem.IsCurrentRoutingProductEditable = (
                    (zoneRoutingProduct.Source == SaleEntityZoneRoutingProductSource.ProductZone && ownerType == SalePriceListOwnerType.SellingProduct) ||
                    (zoneRoutingProduct.Source == SaleEntityZoneRoutingProductSource.CustomerZone && ownerType == SalePriceListOwnerType.Customer)
                );
            }
        }

        private void SetZoneEffectiveRoutingProduct(SalePriceListOwnerType ownerType, int ownerId, ZoneItem zoneItem)
        {
            if (zoneItem.NewRoutingProductId != null)
            {
                zoneItem.EffectiveRoutingProductId = (int)zoneItem.NewRoutingProductId;
                zoneItem.EffectiveRoutingProductName = new RoutingProductManager().GetRoutingProduct(zoneItem.EffectiveRoutingProductId).Name;
            }
            else {
                Changes changes = _dataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);
                
                if (changes != null && changes.DefaultChanges != null && changes.DefaultChanges.NewDefaultRoutingProduct != null) {
                    zoneItem.EffectiveRoutingProductId = changes.DefaultChanges.NewDefaultRoutingProduct.DefaultRoutingProductId;
                    zoneItem.EffectiveRoutingProductName = new RoutingProductManager().GetRoutingProduct(changes.DefaultChanges.NewDefaultRoutingProduct.DefaultRoutingProductId).Name;
                }
                else if (zoneItem.CurrentRoutingProductId != null)
                {
                    zoneItem.EffectiveRoutingProductId = (int)zoneItem.CurrentRoutingProductId;
                    zoneItem.EffectiveRoutingProductName = zoneItem.CurrentRoutingProductName;
                }
            }
        }

        private void SetRouteOptionsForZoneItems(IEnumerable<ZoneItem> zoneItems)
        {
            IEnumerable<RPZone> rpZones = zoneItems.MapRecords(item => new RPZone() { RoutingProductId = item.EffectiveRoutingProductId, SaleZoneId = item.ZoneId });
            IEnumerable<RPRoute> rpRoutes = new RPRouteManager().GetRPRoutes(rpZones);

            foreach (ZoneItem zoneItem in zoneItems)
            {
                RPRoute rpRoute = rpRoutes.FindRecord(item => item.SaleZoneId == zoneItem.ZoneId);
                if (rpRoute != null)
                {
                    IEnumerable<RPRouteOption> routeOptions = new List<RPRouteOption>();
                    rpRoute.RPOptionsByPolicy.TryGetValue(1, out routeOptions);
                    zoneItem.RouteOptions = routeOptions.MapRecords(item => new RPRouteOptionDetail() { Entity = new RPRouteOption() { SupplierId = item.SupplierId, SupplierRate = item.SupplierRate, Percentage = item.Percentage }, SupplierName = new CarrierAccountManager().GetCarrierAccount(item.SupplierId).Name });
                }
            }
        }

        private int GetSellingProductId(int customerId)
        {
            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            CustomerSellingProduct customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(customerId, DateTime.Now, false);
            return customerSellingProduct.SellingProductId;
        }

        #region Set Zone Item Changes

        private void SetZoneItemChanges(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<ZoneItem> zoneItems)
        {
            Changes existingChanges = _dataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);

            if (existingChanges != null && existingChanges.DefaultChanges == null && existingChanges.ZoneChanges == null)
                existingChanges = null;

            if (existingChanges != null && existingChanges.ZoneChanges != null)
            {
                foreach (ZoneChanges zoneItemChanges in existingChanges.ZoneChanges)
                {
                    SetZoneItemRateChanges(zoneItemChanges.NewRate, zoneItemChanges.RateChange, zoneItems);
                    SetZoneItemRoutingProductChanges(zoneItemChanges.NewRoutingProduct, zoneItemChanges.RoutingProductChange, zoneItems);
                }
            }
        }

        private void SetZoneItemRateChanges(NewRate newRate, RateChange rateChange, IEnumerable<ZoneItem> zoneItems)
        {
            if (newRate != null)
            {
                ZoneItem zoneItem = zoneItems.FindRecord(item => item.ZoneId == newRate.ZoneId);

                if (zoneItem != null)
                {
                    zoneItem.NewRate = newRate.NormalRate;
                    zoneItem.NewRateBED = newRate.BED;
                    zoneItem.NewRateEED = newRate.EED;
                }
            }
            else if (rateChange != null)
            {
                ZoneItem zoneItem = zoneItems.FindRecord(item => item.CurrentRateId == rateChange.RateId);

                if (zoneItem != null)
                    zoneItem.NewRateEED = rateChange.EED;
            }
        }

        private void SetZoneItemRoutingProductChanges(NewZoneRoutingProduct newRoutingProduct, ZoneRoutingProductChange routingProductChange, IEnumerable<ZoneItem> zoneItems)
        {
            if (newRoutingProduct != null)
            {
                ZoneItem zoneItem = zoneItems.FindRecord(item => item.ZoneId == newRoutingProduct.ZoneId);

                if (zoneItem != null)
                {
                    zoneItem.NewRoutingProductId = newRoutingProduct.ZoneRoutingProductId;
                    zoneItem.NewRoutingProductBED = newRoutingProduct.BED;
                    zoneItem.NewRoutingProductEED = newRoutingProduct.EED;
                }
            }
            else if (routingProductChange != null)
            {
                ZoneItem zoneItem = zoneItems.FindRecord(item => item.ZoneId == routingProductChange.ZoneRoutingProductId);

                if (zoneItem != null)
                    zoneItem.NewRoutingProductEED = routingProductChange.EED;
            }
        }

        #endregion

        #endregion

        #region Get Default Item

        public DefaultItem GetDefaultItem(SalePriceListOwnerType ownerType, int ownerId)
        {
            DefaultItem defaultItem = new DefaultItem();
            
            SaleEntityZoneRoutingProductLocator locator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(DateTime.Now));
            SaleEntityZoneRoutingProduct locatorResult;
            
            locatorResult = (ownerType == SalePriceListOwnerType.SellingProduct) ?
                locator.GetSellingProductDefaultRoutingProduct(ownerId) :
                locator.GetCustomerDefaultRoutingProduct(ownerId, GetSellingProductId(ownerId));

            if (locatorResult != null)
            {
                defaultItem.CurrentRoutingProductId = locatorResult.RoutingProductId;
                defaultItem.CurrentRoutingProductName = (defaultItem.CurrentRoutingProductId != null) ?
                    new RoutingProductManager().GetRoutingProduct((int)defaultItem.CurrentRoutingProductId).Name : null;
                defaultItem.CurrentRoutingProductBED = locatorResult.BED;
                defaultItem.CurrentRoutingProductEED = locatorResult.EED;

                defaultItem.IsCurrentRoutingProductEditable = (
                    (locatorResult.Source == SaleEntityZoneRoutingProductSource.ProductDefault && ownerType == SalePriceListOwnerType.SellingProduct) ||
                    (locatorResult.Source == SaleEntityZoneRoutingProductSource.CustomerDefault && ownerType == SalePriceListOwnerType.Customer)
                );
            }

            SetDefaultItemChanges(ownerType, ownerId, defaultItem);

            return defaultItem;
        }

        private void SetDefaultItemChanges(SalePriceListOwnerType ownerType, int ownerId, DefaultItem defaultItem)
        {
            Changes existingChanges = _dataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);

            if (existingChanges != null && existingChanges.DefaultChanges == null && existingChanges.ZoneChanges == null)
                existingChanges = null;

            if (existingChanges != null && existingChanges.DefaultChanges != null)
            {
                NewDefaultRoutingProduct newRoutingProduct = existingChanges.DefaultChanges.NewDefaultRoutingProduct;
                DefaultRoutingProductChange routingProductChange = existingChanges.DefaultChanges.DefaultRoutingProductChange;

                if (newRoutingProduct != null)
                {
                    defaultItem.NewRoutingProductId = newRoutingProduct.DefaultRoutingProductId;
                    defaultItem.NewRoutingProductBED = newRoutingProduct.BED;
                    defaultItem.NewRoutingProductEED = newRoutingProduct.EED;
                }
                else if (routingProductChange != null)
                    defaultItem.NewRoutingProductEED = routingProductChange.EED;
            }
        }

        #endregion

        #region Save Price List

        public bool SavePriceList(SalePriceListOwnerType ownerType, int ownerId)
        {
            bool succeeded = false;
            Changes changes = _dataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);

            if (changes != null)
            {
                if (changes.DefaultChanges != null)
                {
                    var newDefaultRoutingProduct = changes.DefaultChanges.NewDefaultRoutingProduct;
                    var defaultRoutingProductChange = changes.DefaultChanges.DefaultRoutingProductChange;

                    ISaleEntityRoutingProductDataManager routingProductDataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();

                    if (newDefaultRoutingProduct != null)
                        routingProductDataManager.InsertOrUpdateDefaultRoutingProduct(ownerType, ownerId, newDefaultRoutingProduct);
                    else
                        routingProductDataManager.UpdateDefaultRoutingProduct(ownerType, ownerId, defaultRoutingProductChange);
                }

                if (changes.ZoneChanges != null)
                {
                    int priceListId = AddPriceList(ownerType, ownerId);

                    List<NewRate> newRates = changes.ZoneChanges.MapRecords(item => item.NewRate, item => item.NewRate != null).ToList();
                    List<SaleRate> newSaleRates = MapNewRatesToSaleRates(newRates, priceListId).ToList();
                    List<RateChange> rateChanges = changes.ZoneChanges.MapRecords(item => item.RateChange, item => item.RateChange != null).ToList();
                    List<NewZoneRoutingProduct> newRoutingProducts = changes.ZoneChanges.MapRecords(item => item.NewRoutingProduct, item => item.NewRoutingProduct != null).ToList();
                    List<ZoneRoutingProductChange> routingProductChanges = changes.ZoneChanges.MapRecords(item => item.RoutingProductChange, item => item.RoutingProductChange != null).ToList();

                    ISaleRateDataManager saleRateDataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();

                    if (rateChanges != null)
                        succeeded = saleRateDataManager.CloseRates(rateChanges);

                    if (newSaleRates != null)
                        succeeded = saleRateDataManager.InsertRates(newSaleRates);

                    ISaleEntityRoutingProductDataManager saleEntityRoutingProductDataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();

                    if (routingProductChanges != null)
                        saleEntityRoutingProductDataManager.UpdateZoneRoutingProducts(ownerType, ownerId, routingProductChanges);

                    if (newRoutingProducts != null)
                        saleEntityRoutingProductDataManager.InsertOrUpdateZoneRoutingProducts(ownerType, ownerId, newRoutingProducts);
                }

                _dataManager.sp_RatePlan_SetStatusIfExists(ownerType, ownerId, RatePlanStatus.Completed);
            }

            return succeeded;
        }

        public int AddPriceList(SalePriceListOwnerType ownerType, int ownerId)
        {
            int priceListId;

            SalePriceList priceList = new SalePriceList()
            {
                OwnerType = ownerType,
                OwnerId = ownerId
            };

            bool added = _dataManager.InsertPriceList(priceList, out priceListId);

            return priceListId;
        }

        private IEnumerable<SaleRate> MapNewRatesToSaleRates(IEnumerable<NewRate> newRates, int priceListId)
        {
            List<SaleRate> saleRates = new List<SaleRate>();

            foreach (NewRate newRate in newRates)
            {
                saleRates.Add(new SaleRate()
                {
                    ZoneId = newRate.ZoneId,
                    PriceListId = priceListId,
                    CurrencyId = newRate.CurrencyId,
                    NormalRate = newRate.NormalRate,
                    BeginEffectiveDate = newRate.BED,
                    EndEffectiveDate = newRate.EED
                });
            }

            return saleRates;
        }

        #endregion

        #region Save Changes

        public bool SaveChanges(SaveChangesInput input)
        {
            Changes existingChanges = _dataManager.GetChanges(input.OwnerType, input.OwnerId, RatePlanStatus.Draft);
            Changes allChanges = MergeChanges(existingChanges, input.NewChanges);

            return _dataManager.InsertOrUpdateChanges(input.OwnerType, input.OwnerId, allChanges, RatePlanStatus.Draft);
        }

        private Changes MergeChanges(Changes existingChanges, Changes newChanges)
        {
            return Merge(existingChanges, newChanges,
                () =>
                {
                    Changes allChanges = new Changes();

                    allChanges.ZoneChanges = MergeZoneChanges(existingChanges.ZoneChanges, newChanges.ZoneChanges);
                    allChanges.DefaultChanges = (newChanges.DefaultChanges != null && (newChanges.DefaultChanges.NewDefaultRoutingProduct != null || newChanges.DefaultChanges.DefaultRoutingProductChange != null)) ? newChanges.DefaultChanges : null;

                    if ((allChanges.ZoneChanges == null || allChanges.ZoneChanges.Count == 0) && allChanges.DefaultChanges == null)
                        allChanges = null;

                    return allChanges;
                });
        }

        private List<ZoneChanges> MergeZoneChanges(List<ZoneChanges> existingZoneChanges, List<ZoneChanges> newZoneChanges)
        {
            return Merge(existingZoneChanges, newZoneChanges,
                () =>
                {
                    foreach (ZoneChanges zoneItemChanges in existingZoneChanges)
                    {
                        if (!newZoneChanges.Any(item => item.ZoneId == zoneItemChanges.ZoneId))
                            newZoneChanges.Add(zoneItemChanges);
                    }

                    newZoneChanges.RemoveAll(item => item.NewRate == null && item.RateChange == null && item.NewRoutingProduct == null && item.RoutingProductChange == null);

                    return newZoneChanges;
                });
        }

        private T Merge<T>(T existingChanges, T newChanges, Func<T> mergeLogic) where T : class
        {
            if (existingChanges != null && newChanges != null)
                return mergeLogic();

            return existingChanges != null ? existingChanges : newChanges;
        }

        #endregion
    }
}
