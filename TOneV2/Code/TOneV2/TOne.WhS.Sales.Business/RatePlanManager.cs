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
using TOne.WhS.Sales.Entities.RateManagement;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

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
                        Changes changes = _dataManager.GetChanges(input.Filter.OwnerType, input.Filter.OwnerId, RatePlanStatus.Draft);

                        foreach (SaleZone zone in zones)
                        {
                            ZoneItem zoneItem = new ZoneItem();

                            zoneItem.ZoneId = zone.SaleZoneId;
                            zoneItem.ZoneName = zone.Name;

                            SetZoneItemRate(input.Filter.OwnerType, input.Filter.OwnerId, zoneItem);
                            SetZoneItemRoutingProduct(input.Filter.OwnerType, input.Filter.OwnerId, zoneItem);

                            if (changes != null && changes.ZoneChanges != null)
                            {
                                ZoneChanges zoneChanges = changes.ZoneChanges.FindRecord(item => item.ZoneId == zoneItem.ZoneId);
                                if (zoneChanges != null)
                                    SetZoneItemChanges(zoneItem, zoneChanges);
                            }

                            NewDefaultRoutingProduct newDefaultRoutingProduct = (changes != null && changes.DefaultChanges != null) ? changes.DefaultChanges.NewDefaultRoutingProduct : null;
                            SetZoneItemEffectiveRoutingProduct(zoneItem, newDefaultRoutingProduct);

                            zoneItems.Add(zoneItem);
                        }

                        SetRouteOptionsAndCostsForZoneItems(input.Filter.RoutingDatabaseId, input.Filter.RPRoutePolicyConfigId, input.Filter.NumberOfOptions, zoneItems, input.Filter.CostCalculationMethods);
                    }
                }
            }

            return zoneItems;
        }

        public ZoneItem GetZoneItem(SalePriceListOwnerType ownerType, int ownerId, int routingDatabaseId, int policyConfigId, int numberOfOptions, long zoneId)
        {
            ZoneItem zoneItem = new ZoneItem() { ZoneId = zoneId };
            zoneItem.ZoneName = GetZoneName(zoneId);

            SetZoneItemRate(ownerType, ownerId, zoneItem);
            SetZoneItemRoutingProduct(ownerType, ownerId, zoneItem);

            Changes changes = _dataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);
            if (changes != null && changes.ZoneChanges != null)
            {
                ZoneChanges zoneChanges = changes.ZoneChanges.FindRecord(item => item.ZoneId == zoneId);
                if (zoneChanges != null)
                    SetZoneItemChanges(zoneItem, zoneChanges);
            }

            NewDefaultRoutingProduct newDefaultRoutingProduct = (changes != null && changes.DefaultChanges != null) ? changes.DefaultChanges.NewDefaultRoutingProduct : null;
            SetZoneItemEffectiveRoutingProduct(zoneItem, newDefaultRoutingProduct);

            IEnumerable<RPRouteDetail> rpRoutes = GetRPRoutes(routingDatabaseId, policyConfigId, numberOfOptions, new List<ZoneItem>() { zoneItem });
            if (rpRoutes != null && rpRoutes.Count() > 0)
                zoneItem.RouteOptions = rpRoutes.ElementAt(0).RouteOptionsDetails;

            return zoneItem;
        }

        //TODO: Invoke from GetZoneItems
        private string GetZoneName(long zoneId)
        {
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            SaleZone saleZone = saleZoneManager.GetSaleZone(zoneId); // I'm assuming that the sale zone manager only gets effective sale zones
            return saleZone != null ? saleZone.Name : null;
        }

        private IEnumerable<SaleZone> GetZones(SalePriceListOwnerType ownerType, int ownerId)
        {
            IEnumerable<SaleZone> zones = null;

            if (ownerType == SalePriceListOwnerType.SellingProduct)
                zones = GetSellingProductZones(ownerId, DateTime.Now);
            else if (ownerType == SalePriceListOwnerType.Customer)
            {
                CustomerZoneManager manager = new CustomerZoneManager();
                zones = manager.GetCustomerSaleZones(ownerId, DateTime.Now, false);
            }

            return zones;
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
            return saleZones.FindAllRecords(item => item.Name != null && item.Name.Length > 0 && char.ToLower(item.Name.ElementAt(0)) == char.ToLower(filter.ZoneLetter));
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

        private void SetZoneItemRate(SalePriceListOwnerType ownerType, int ownerId, ZoneItem zoneItem)
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
                zoneItem.CurrentRateBED = zoneRate.Rate.BED;
                zoneItem.CurrentRateEED = zoneRate.Rate.EED;
            }
        }

        private void SetZoneItemRoutingProduct(SalePriceListOwnerType ownerType, int ownerId, ZoneItem zoneItem)
        {
            SaleEntityZoneRoutingProductLocator locator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(DateTime.Now));

            SaleEntityZoneRoutingProduct zoneRoutingProduct = (ownerType == SalePriceListOwnerType.SellingProduct) ?
                locator.GetSellingProductZoneRoutingProduct(ownerId, zoneItem.ZoneId) :
                locator.GetCustomerZoneRoutingProduct(ownerId, GetSellingProductId(ownerId), zoneItem.ZoneId);

            if (zoneRoutingProduct != null)
            {
                zoneItem.CurrentRoutingProductId = zoneRoutingProduct.RoutingProductId;
                zoneItem.CurrentRoutingProductName = zoneItem.CurrentRoutingProductId != null ? GetRoutingProductName((int)zoneItem.CurrentRoutingProductId) : null;

                zoneItem.CurrentRoutingProductBED = zoneRoutingProduct.BED;
                zoneItem.CurrentRoutingProductEED = zoneRoutingProduct.EED;

                zoneItem.IsCurrentRoutingProductEditable = (
                    (zoneRoutingProduct.Source == SaleEntityZoneRoutingProductSource.ProductZone && ownerType == SalePriceListOwnerType.SellingProduct) ||
                    (zoneRoutingProduct.Source == SaleEntityZoneRoutingProductSource.CustomerZone && ownerType == SalePriceListOwnerType.Customer)
                );
            }
        }

        private void SetZoneItemEffectiveRoutingProduct(ZoneItem zoneItem, NewDefaultRoutingProduct newDefaultRoutingProduct)
        {
            if (zoneItem.NewRoutingProductId != null)
            {
                zoneItem.EffectiveRoutingProductId = (int)zoneItem.NewRoutingProductId;
                zoneItem.EffectiveRoutingProductName = GetRoutingProductName(zoneItem.EffectiveRoutingProductId);
            }
            else {
                if (newDefaultRoutingProduct != null)
                {
                    zoneItem.EffectiveRoutingProductId = newDefaultRoutingProduct.DefaultRoutingProductId;
                    zoneItem.EffectiveRoutingProductName = GetRoutingProductName(newDefaultRoutingProduct.DefaultRoutingProductId);
                }
                else if (zoneItem.CurrentRoutingProductId != null)
                {
                    zoneItem.EffectiveRoutingProductId = (int)zoneItem.CurrentRoutingProductId;
                    zoneItem.EffectiveRoutingProductName = zoneItem.CurrentRoutingProductName;
                }
            }
        }

        private void SetRouteOptionsAndCostsForZoneItems(int routingDatabaseId, int policyConfigId, int numberOfOptions, IEnumerable<ZoneItem> zoneItems, IEnumerable<CostCalculationMethod> costCalculationMethods)
        {
            IEnumerable<RPRouteDetail> rpRoutes = GetRPRoutes(routingDatabaseId, policyConfigId, numberOfOptions, zoneItems);

            if (rpRoutes != null)
            {
                foreach (RPRouteDetail rpRoute in rpRoutes)
                {
                    ZoneItem zoneItem = zoneItems.FindRecord(itm => itm.ZoneId == rpRoute.SaleZoneId);

                    if (zoneItem != null)
                    {
                        zoneItem.RouteOptions = rpRoute.RouteOptionsDetails;

                        if (costCalculationMethods != null && costCalculationMethods.Count() > 0)
                        {
                            zoneItem.Costs = new List<decimal>();

                            foreach (CostCalculationMethod method in costCalculationMethods)
                            {
                                CostCalculationMethodContext context = new CostCalculationMethodContext() { Route = rpRoute };
                                method.CalculateCost(context);
                                zoneItem.Costs.Add(context.Cost);
                            }
                        }
                    }
                }
            }
        }

        private IEnumerable<RPRouteDetail> GetRPRoutes(int routingDatabaseId, int policyConfigId, int numberOfOptions, IEnumerable<ZoneItem> zoneItems)
        {
            IEnumerable<RPZone> rpZones = zoneItems.MapRecords(item => new RPZone()
            {
                RoutingProductId = item.EffectiveRoutingProductId,
                SaleZoneId = item.ZoneId
            });
            return new RPRouteManager().GetRPRoutes(routingDatabaseId, policyConfigId, numberOfOptions, rpZones);
        }

        //TODO: Handle the case when a customer isn't associated with a selling product
        private int GetSellingProductId(int customerId)
        {
            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            CustomerSellingProduct customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(customerId, DateTime.Now, false);
            
            return customerSellingProduct != null ? customerSellingProduct.SellingProductId : -1;
        }

        private string GetRoutingProductName(int routingProductId)
        {
            RoutingProductManager routingProductManager = new RoutingProductManager();
            RoutingProduct routingProduct = routingProductManager.GetRoutingProduct(routingProductId);
            
            return routingProduct != null ? routingProduct.Name : null;
        }

        private string GetCarrierAccountName(int carrierAccountId)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            CarrierAccount carrierAccount = carrierAccountManager.GetCarrierAccount(carrierAccountId);

            return carrierAccount != null ? carrierAccount.Name : null;
        }

        public List<TemplateConfig> GetCostCalculationMethodTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CostCalculationMethod);
        }

        #region Set Zone Item Changes

        private void SetChangesForZoneItems(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<ZoneItem> zoneItems)
        {
            Changes changes = _dataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);

            if (changes != null && changes.ZoneChanges != null)
            {
                foreach (ZoneChanges zoneItemChanges in changes.ZoneChanges)
                {
                    ZoneItem zoneItem = zoneItems.FindRecord(item => item.ZoneId == zoneItemChanges.ZoneId);
                    if (zoneItem != null)
                        SetZoneItemChanges(zoneItem, zoneItemChanges);
                }
            }
        }

        private void SetZoneItemChanges(ZoneItem zoneItem, ZoneChanges zoneChanges)
        {
            SetZoneItemRateChanges(zoneItem, zoneChanges.NewRate, zoneChanges.RateChange);
            SetZoneItemRoutingProductChanges(zoneItem, zoneChanges.NewRoutingProduct, zoneChanges.RoutingProductChange);
        }

        private void SetZoneItemRateChanges(ZoneItem zoneItem, Entities.NewRate newRate, RateChange rateChange)
        {
            if (newRate != null)
            {
                zoneItem.NewRate = newRate.NormalRate;
                zoneItem.NewRateBED = newRate.BED;
                zoneItem.NewRateEED = newRate.EED;
            }
            else if (rateChange != null)
                zoneItem.NewRateEED = rateChange.EED;
        }

        private void SetZoneItemRoutingProductChanges(ZoneItem zoneItem, NewZoneRoutingProduct newRoutingProduct, ZoneRoutingProductChange routingProductChange)
        {
            if (newRoutingProduct != null)
            {
                zoneItem.NewRoutingProductId = newRoutingProduct.ZoneRoutingProductId;
                zoneItem.NewRoutingProductBED = newRoutingProduct.BED;
                zoneItem.NewRoutingProductEED = newRoutingProduct.EED;
            }
            else if (routingProductChange != null)
                zoneItem.NewRoutingProductEED = routingProductChange.EED;
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

                    IEnumerable<Entities.NewRate> newRates = changes.ZoneChanges.MapRecords(itm => itm.NewRate, itm => itm.NewRate != null);
                    PriceListRateManager rateManager = new PriceListRateManager();
                    
                    Func<Entities.NewRate, Entities.RateManagement.NewRate> mappingExpression = (itm) =>
                    {
                        Entities.RateManagement.NewRate newRate = new Entities.RateManagement.NewRate()
                        {
                            ZoneId = itm.ZoneId,
                            NormalRate = itm.NormalRate,
                            OtherRates = itm.OtherRates,
                            CurrencyId = itm.CurrencyId,
                            BED = itm.BED,
                            EED = itm.EED
                        };

                        IEnumerable<ExistingRate> existingRates = _dataManager.GetZoneExistingRates(ownerType, ownerId, newRate.ZoneId, newRate.BED);
                        rateManager.ProcessNewRate(newRate, existingRates);

                        return newRate;
                    };

                    IEnumerable<Entities.RateManagement.NewRate> managerNewRates = newRates.MapRecords(mappingExpression);

                    List<RateChange> rateChanges = new List<RateChange>();
                    foreach (Entities.RateManagement.NewRate newRate in managerNewRates)
                    {
                        if (newRate.ChangedExistingRates != null && newRate.ChangedExistingRates.Count > 0)
                        {
                            foreach (ExistingRate existingRate in newRate.ChangedExistingRates)
                                rateChanges.Add(new RateChange() { RateId = existingRate.ChangedRate.RateId, EED = existingRate.ChangedRate.EED });
                        }
                    }
                    
                    IEnumerable<NewZoneRoutingProduct> newRoutingProducts = changes.ZoneChanges.MapRecords(item => item.NewRoutingProduct, item => item.NewRoutingProduct != null);
                    IEnumerable<ZoneRoutingProductChange> routingProductChanges = changes.ZoneChanges.MapRecords(item => item.RoutingProductChange, item => item.RoutingProductChange != null);

                    ISaleRateDataManager saleRateDataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();

                    if (rateChanges != null)
                        succeeded = saleRateDataManager.CloseRates(rateChanges);

                    //if (newSaleRates != null)
                    //    succeeded = saleRateDataManager.InsertRates(newSaleRates);

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

        private IEnumerable<SaleRate> MapNewRatesToSaleRates(IEnumerable<TOne.WhS.Sales.Entities.NewRate> newRates, int priceListId)
        {
            List<SaleRate> saleRates = new List<SaleRate>();

            foreach (Entities.NewRate newRate in newRates)
            {
                saleRates.Add(new SaleRate()
                {
                    ZoneId = newRate.ZoneId,
                    PriceListId = priceListId,
                    CurrencyId = newRate.CurrencyId,
                    NormalRate = newRate.NormalRate,
                    BED = newRate.BED,
                    EED = newRate.EED
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

                    allChanges.DefaultChanges = newChanges.DefaultChanges == null ? existingChanges.DefaultChanges : newChanges.DefaultChanges;
                    allChanges.ZoneChanges = MergeZoneChanges(existingChanges.ZoneChanges, newChanges.ZoneChanges);

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
