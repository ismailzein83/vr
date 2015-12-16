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
                    zoneLetters = saleZones.MapRecords(itm => char.ToUpper(itm.Name[0]), itm => itm.Name != null && itm.Name.Length > 0).Distinct().OrderBy(itm => itm);
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

        public IEnumerable<ZoneItem> GetZoneItems(ZoneItemsInput input)
        {
            List<ZoneItem> zoneItems = null;
            IEnumerable<SaleZone> zones = GetZones(input.Filter.OwnerType, input.Filter.OwnerId);

            if (zones != null)
            {
                zones = GetFilteredZones(zones, input.Filter);

                if (zones != null)
                {
                    if (input.Filter.ZoneIds == null)
                        zones = GetPagedZones(zones, input.FromRow, input.ToRow);

                    if (zones != null)
                    {
                        zoneItems = new List<ZoneItem>();
                        Changes changes = input.Filter.ZoneIds == null ? _dataManager.GetChanges(input.Filter.OwnerType, input.Filter.OwnerId, RatePlanStatus.Draft) : null;

                        foreach (SaleZone zone in zones)
                        {
                            ZoneItem zoneItem = new ZoneItem();
                            NewDefaultRoutingProduct newDefaultRoutingProduct = null;
                            ZoneRoutingProductChange zoneRoutingProductChange = null;

                            zoneItem.ZoneId = zone.SaleZoneId;
                            zoneItem.ZoneName = zone.Name;

                            if (input.Filter.ZoneIds == null)
                            {
                                ZoneChanges zoneChanges = null;

                                if (changes != null && changes.ZoneChanges != null)
                                {
                                    zoneChanges = changes.ZoneChanges.FindRecord(itm => itm.ZoneId == zoneItem.ZoneId);
                                    if (zoneChanges != null)
                                        SetZoneItemChanges(zoneItem, zoneChanges);
                                }

                                newDefaultRoutingProduct = (changes != null && changes.DefaultChanges != null) ? changes.DefaultChanges.NewDefaultRoutingProduct : null;
                                zoneRoutingProductChange = zoneChanges != null ? zoneChanges.RoutingProductChange : null;
                            }

                            SetZoneItemRate(input.Filter.OwnerType, input.Filter.OwnerId, zoneItem);
                            SetZoneItemRoutingProduct(input.Filter.OwnerType, input.Filter.OwnerId, zoneItem, newDefaultRoutingProduct, zoneRoutingProductChange);

                            zoneItems.Add(zoneItem);
                        }

                        if (input.Filter.ZoneIds == null)
                            SetRouteOptionsAndCostsForZoneItems(input.Filter.RoutingDatabaseId, input.Filter.RPRoutePolicyConfigId, input.Filter.NumberOfOptions, zoneItems, input.Filter.CostCalculationMethods);
                    }
                }
            }

            return zoneItems;
        }

        public ZoneItem GetZoneItem(ZoneItemInput input)
        {
            ZoneItem zoneItem = new ZoneItem() { ZoneId = input.ZoneId };
            zoneItem.ZoneName = GetZoneName(input.ZoneId);

            Changes changes = _dataManager.GetChanges(input.OwnerType, input.OwnerId, RatePlanStatus.Draft);
            NewDefaultRoutingProduct newDefaultRoutingProduct = (changes != null && changes.DefaultChanges != null) ? changes.DefaultChanges.NewDefaultRoutingProduct : null;
            ZoneChanges zoneChanges = null;
            ZoneRoutingProductChange zoneRoutingProductChange = null;

            if (changes != null && changes.ZoneChanges != null)
            {
                zoneChanges = changes.ZoneChanges.FindRecord(itm => itm.ZoneId == input.ZoneId);

                if (zoneChanges != null)
                {
                    SetZoneItemChanges(zoneItem, zoneChanges);
                    zoneRoutingProductChange = zoneChanges.RoutingProductChange;
                }
            }

            SetZoneItemRate(input.OwnerType, input.OwnerId, zoneItem);
            SetZoneItemRoutingProduct(input.OwnerType, input.OwnerId, zoneItem, newDefaultRoutingProduct, zoneRoutingProductChange);

            IEnumerable<RPRouteDetail> rpRoutes = GetRPRoutes(input.RoutingDatabaseId, input.PolicyConfigId, input.NumberOfOptions, new List<ZoneItem>() { zoneItem });
            if (rpRoutes != null && rpRoutes.Count() > 0)
            {
                zoneItem.RouteOptions = rpRoutes.ElementAt(0).RouteOptionsDetails;

                if (input.CostCalculationMethods != null && input.CostCalculationMethods.Count() > 0)
                {
                    zoneItem.Costs = new List<decimal>();

                    foreach (CostCalculationMethod method in input.CostCalculationMethods)
                    {
                        CostCalculationMethodContext context = new CostCalculationMethodContext() { Route = rpRoutes.ElementAt(0) };
                        method.CalculateCost(context);
                        zoneItem.Costs.Add(context.Cost);
                    }
                }
            }

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
            IEnumerable<SaleZone> zones = null;

            SellingProductManager sellingProductManager = new SellingProductManager();
            int? returnValue = sellingProductManager.GetSellingNumberPlanId(sellingProductId);
            
            if (returnValue != null)
            {
                int sellingNumberPlanId = (int)returnValue;
                SaleZoneManager saleZoneManager = new SaleZoneManager();
                zones = saleZoneManager.GetSaleZones(sellingNumberPlanId, effectiveOn);
            }

            return zones;
        }

        private IEnumerable<SaleZone> GetFilteredZones(IEnumerable<SaleZone> saleZones, ZoneItemFilter filter)
        {
            if (filter.ZoneIds != null)
                return saleZones.FindAllRecords(itm => filter.ZoneIds.Contains(itm.SaleZoneId));
            return saleZones.FindAllRecords(itm => itm.Name != null && itm.Name.Length > 0 && char.ToLower(itm.Name.ElementAt(0)) == char.ToLower(filter.ZoneLetter));
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

        private void SetZoneItemRoutingProduct(SalePriceListOwnerType ownerType, int ownerId, ZoneItem zoneItem, NewDefaultRoutingProduct newDefaultRoutingProduct, ZoneRoutingProductChange zoneRoutingProductChange)
        {
            SaleEntityZoneRoutingProductLocator locator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(DateTime.Now));

            SaleEntityZoneRoutingProduct zoneRoutingProduct = (ownerType == SalePriceListOwnerType.SellingProduct) ?
                locator.GetSellingProductZoneRoutingProduct(ownerId, zoneItem.ZoneId) :
                locator.GetCustomerZoneRoutingProduct(ownerId, GetSellingProductId(ownerId), zoneItem.ZoneId);

            SaleEntityZoneRoutingProduct currentDefaultRoutingProduct = (ownerType == SalePriceListOwnerType.SellingProduct) ?
                locator.GetSellingProductDefaultRoutingProduct(ownerId) : locator.GetCustomerDefaultRoutingProduct(ownerId, GetSellingProductId(ownerId));

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

            SetZoneItemEffectiveRoutingProduct(zoneItem, zoneRoutingProductChange, newDefaultRoutingProduct, zoneRoutingProduct, currentDefaultRoutingProduct);
        }

        private void SetZoneItemEffectiveRoutingProduct(ZoneItem zoneItem, ZoneRoutingProductChange zoneRoutingProductChange, NewDefaultRoutingProduct newDefaultRoutingProduct, SaleEntityZoneRoutingProduct zoneRoutingProduct, SaleEntityZoneRoutingProduct currentDefaultRoutingProduct)
        {
            if (zoneItem.NewRoutingProductId != null)
            {
                zoneItem.EffectiveRoutingProductId = (int)zoneItem.NewRoutingProductId;
                zoneItem.EffectiveRoutingProductName = GetRoutingProductName(zoneItem.EffectiveRoutingProductId);
            }
            else if (zoneRoutingProductChange != null)
                SetZoneItemEffectiveRoutingProductToDefault(zoneItem, newDefaultRoutingProduct, currentDefaultRoutingProduct);
            else if (zoneRoutingProduct != null && (zoneRoutingProduct.Source == SaleEntityZoneRoutingProductSource.ProductZone || zoneRoutingProduct.Source == SaleEntityZoneRoutingProductSource.CustomerZone))
            {
                zoneItem.EffectiveRoutingProductId = zoneRoutingProduct.RoutingProductId;
                zoneItem.EffectiveRoutingProductName = GetRoutingProductName(zoneItem.EffectiveRoutingProductId);
            }
            else
                SetZoneItemEffectiveRoutingProductToDefault(zoneItem, newDefaultRoutingProduct, currentDefaultRoutingProduct);
        }

        private void SetZoneItemEffectiveRoutingProductToDefault(ZoneItem zoneItem, NewDefaultRoutingProduct newDefaultRoutingProduct, SaleEntityZoneRoutingProduct currentDefaultRoutingProduct)
        {
            if (newDefaultRoutingProduct != null)
            {
                zoneItem.EffectiveRoutingProductId = newDefaultRoutingProduct.DefaultRoutingProductId;
                zoneItem.EffectiveRoutingProductName = GetRoutingProductName(zoneItem.EffectiveRoutingProductId);
            }
            else if (currentDefaultRoutingProduct != null)
            {
                zoneItem.EffectiveRoutingProductId = currentDefaultRoutingProduct.RoutingProductId;
                zoneItem.EffectiveRoutingProductName = GetRoutingProductName(zoneItem.EffectiveRoutingProductId);
            }
        }

        private void SetRouteOptionsAndCostsForZoneItems(int routingDatabaseId, int policyConfigId, int numberOfOptions, IEnumerable<ZoneItem> zoneItems, IEnumerable<CostCalculationMethod> costCalculationMethods)
        {
            IEnumerable<RPRouteDetail> rpRoutes = GetRPRoutes(routingDatabaseId, policyConfigId, numberOfOptions, zoneItems);

            if (rpRoutes != null)
            {
                foreach (RPRouteDetail rpRoute in rpRoutes)
                {
                    if (rpRoute.RouteOptionsDetails != null)
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

        public Vanrise.Entities.IDataRetrievalResult<ZoneRateChangesDetail> GetFilteredZoneRateChanges(Vanrise.Entities.DataRetrievalInput<ZoneRateChangesQuery> input)
        {
            List<ZoneRateChangesDetail> details = null;
            Changes changes = _dataManager.GetChanges(input.Query.OwnerType, input.Query.OwnerId, RatePlanStatus.Draft);

            if (changes != null && changes.ZoneChanges != null)
            {
                IEnumerable<ZoneChanges> zoneChanges = changes.ZoneChanges.FindAllRecords(itm => itm.NewRate != null || itm.RateChange != null);

                if (zoneChanges != null)
                {
                    details = new List<ZoneRateChangesDetail>();
                    SaleEntityZoneRateLocator locator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(DateTime.Now));

                    foreach (ZoneChanges zoneItmChanges in zoneChanges)
                    {
                        ZoneRateChangesDetail detail = new ZoneRateChangesDetail();

                        detail.ZoneId = zoneItmChanges.ZoneId;
                        detail.ZoneName = GetZoneName(zoneItmChanges.ZoneId);

                        SaleEntityZoneRate rate = (input.Query.OwnerType == SalePriceListOwnerType.SellingProduct) ?
                            locator.GetSellingProductZoneRate(input.Query.OwnerId, zoneItmChanges.ZoneId) :
                            locator.GetCustomerZoneRate(input.Query.OwnerId, GetSellingProductId(input.Query.OwnerId), zoneItmChanges.ZoneId);

                        if (rate != null)
                        {
                            detail.CurrentRate = rate.Rate.NormalRate;
                            detail.IsCurrentRateInherited = (input.Query.OwnerType == SalePriceListOwnerType.Customer && rate.Source == SalePriceListOwnerType.SellingProduct);
                        }

                        if (zoneItmChanges.NewRate != null)
                        {
                            detail.NewRate = zoneItmChanges.NewRate.NormalRate;
                            detail.EffectiveOn = zoneItmChanges.NewRate.BED;
                            detail.EffectiveUntil = zoneItmChanges.NewRate.EED;
                        }
                        else if (zoneItmChanges.RateChange != null)
                            detail.EffectiveOn = zoneItmChanges.RateChange.EED;

                        if (detail.CurrentRate != null && detail.NewRate != null)
                        {
                            if (detail.NewRate > detail.CurrentRate)
                                detail.ChangeType = Entities.RateChangeType.Increase;
                            else if (detail.NewRate < detail.CurrentRate)
                                detail.ChangeType = Entities.RateChangeType.Decrease;
                        }
                        else if (detail.NewRate != null)
                            detail.ChangeType = Entities.RateChangeType.New;
                        else
                            detail.ChangeType = Entities.RateChangeType.Close;

                        details.Add(detail);
                    }
                }
            }

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, details.ToBigResult(input, null));
        }
        public Vanrise.Entities.IDataRetrievalResult<ZoneRoutingProductChangesDetail> GetFilteredZoneRoutingProductChanges(Vanrise.Entities.DataRetrievalInput<ZoneRoutingProductChangesQuery> input)
        {
            List<ZoneRoutingProductChangesDetail> details = null;
            Changes changes = _dataManager.GetChanges(input.Query.OwnerType, input.Query.OwnerId, RatePlanStatus.Draft);

            if (changes != null && changes.ZoneChanges != null)
            {
                IEnumerable<ZoneChanges> zoneChanges = changes.ZoneChanges.FindAllRecords(itm => itm.NewRoutingProduct != null || itm.RoutingProductChange != null);

                if (zoneChanges != null)
                {
                    details = new List<ZoneRoutingProductChangesDetail>();
                    SaleEntityZoneRoutingProductLocator locator = new SaleEntityZoneRoutingProductLocator(new SaleEntityRoutingProductReadWithCache(DateTime.Now));
                    
                    DefaultItem defaultItem = GetDefaultItem(input.Query.OwnerType, input.Query.OwnerId);
                    int? currentDefaultRoutingProductId = null;
                    if (defaultItem != null && defaultItem.CurrentRoutingProductId != null)
                        currentDefaultRoutingProductId = (int)defaultItem.CurrentRoutingProductId;

                    foreach (ZoneChanges zoneItmChanges in zoneChanges)
                    {
                        ZoneRoutingProductChangesDetail detail = new ZoneRoutingProductChangesDetail();

                        detail.ZoneId = zoneItmChanges.ZoneId;
                        detail.ZoneName = GetZoneName(zoneItmChanges.ZoneId);

                        SaleEntityZoneRoutingProduct routingProduct = (input.Query.OwnerType == SalePriceListOwnerType.SellingProduct) ?
                            locator.GetSellingProductZoneRoutingProduct(input.Query.OwnerId, zoneItmChanges.ZoneId) :
                            locator.GetCustomerZoneRoutingProduct(input.Query.OwnerId, GetSellingProductId(input.Query.OwnerId), zoneItmChanges.ZoneId);

                        detail.CurrentRoutingProductName = routingProduct != null ? GetRoutingProductName(routingProduct.RoutingProductId) : null;
                        detail.IsCurrentRoutingProductInherited = (currentDefaultRoutingProductId != null && routingProduct.RoutingProductId == (int)currentDefaultRoutingProductId);

                        if (zoneItmChanges.NewRoutingProduct != null)
                        {
                            detail.NewRoutingProductName = GetRoutingProductName(zoneItmChanges.NewRoutingProduct.ZoneRoutingProductId);
                            detail.EffectiveOn = zoneItmChanges.NewRoutingProduct.BED;
                            detail.IsNewRoutingProductInherited = (currentDefaultRoutingProductId != null && zoneItmChanges.NewRoutingProduct.ZoneRoutingProductId == (int)currentDefaultRoutingProductId);
                        }
                        else if (zoneItmChanges.RoutingProductChange != null)
                            detail.EffectiveOn = zoneItmChanges.RoutingProductChange.EED;

                        details.Add(detail);
                    }
                }
            }

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, details.ToBigResult(input, null));
        }

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

                defaultItem.IsCurrentRoutingProductInherited = (ownerType == SalePriceListOwnerType.Customer && locatorResult.Source == SaleEntityZoneRoutingProductSource.ProductDefault);
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
                    else if (defaultRoutingProductChange != null)
                        routingProductDataManager.UpdateDefaultRoutingProduct(ownerType, ownerId, defaultRoutingProductChange);
                }

                if (changes.ZoneChanges != null)
                {
                    PriceListRateManager rateManager = new PriceListRateManager();
                    ISaleRateDataManager saleRateDataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
                    ISaleEntityRoutingProductDataManager saleEntityRoutingProductDataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();

                    IEnumerable<NewRate> newRates = null;
                    List<RateChange> rateChanges = null;
                    IEnumerable<NewZoneRoutingProduct> newRoutingProducts = null;
                    IEnumerable<ZoneRoutingProductChange> routingProductChanges = null;

                    int priceListId = AddPriceList(ownerType, ownerId);
                    newRates = changes.ZoneChanges.MapRecords(itm => itm.NewRate, itm => itm.NewRate != null);

                    if (newRates != null && newRates.Count() > 0)
                    {
                        DateTime minBED = newRates.MapRecords(itm => itm.BED).OrderBy(itm => itm).FirstOrDefault();
                        IEnumerable<long> zoneIds = newRates.MapRecords(itm => itm.ZoneId);
                        IEnumerable<SaleRate> existingRates = saleRateDataManager.GetExistingRatesByZoneIds(ownerType, ownerId, zoneIds, minBED);
                        
                        foreach (NewRate newRate in newRates)
                        {
                            IEnumerable<SaleRate> zoneExistingRates = existingRates.FindAllRecords(itm => itm.ZoneId == newRate.ZoneId && itm.IsOverlapedWith(newRate));
                            rateManager.ProcessNewRate(newRate, zoneExistingRates.MapRecords(itm => new ExistingRate() { RateEntity = itm }));
                        }

                        rateChanges = new List<RateChange>();
                        foreach (NewRate newRate in newRates)
                            rateChanges.AddRange(newRate.ChangedExistingRates.MapRecords(itm => new RateChange() { RateId = itm.ChangedRate.RateId, EED = itm.ChangedRate.EED }));
                    }
                    
                    newRoutingProducts = changes.ZoneChanges.MapRecords(itm => itm.NewRoutingProduct, itm => itm.NewRoutingProduct != null);
                    routingProductChanges = changes.ZoneChanges.MapRecords(itm => itm.RoutingProductChange, itm => itm.RoutingProductChange != null);

                    if (rateChanges != null && rateChanges.Count > 0)
                        succeeded = saleRateDataManager.CloseRates(rateChanges);

                    if (newRates != null && newRates.Count() > 0)
                        succeeded = saleRateDataManager.InsertRates(newRates, priceListId);

                    if (routingProductChanges != null && routingProductChanges.Count() > 0)
                        saleEntityRoutingProductDataManager.UpdateZoneRoutingProducts(ownerType, ownerId, routingProductChanges);

                    if (newRoutingProducts != null && newRoutingProducts.Count() > 0)
                        saleEntityRoutingProductDataManager.InsertZoneRoutingProducts(ownerType, ownerId, newRoutingProducts);
                }

                _dataManager.SetRatePlanStatusIfExists(ownerType, ownerId, RatePlanStatus.Completed);
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

        #endregion

        public ChangesDetail GetChanges(SalePriceListOwnerType ownerType, int ownerId)
        {
            ChangesDetail detailedChanges = null;
            Changes changes = _dataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);

            if (changes != null)
            {
                detailedChanges = new ChangesDetail();

                if (changes.DefaultChanges != null)
                {
                    DefaultChangesDetail defaultChanges = new DefaultChangesDetail() { Entity = changes.DefaultChanges };
                    var newDefaultRoutingProduct = defaultChanges.Entity.NewDefaultRoutingProduct;
                    var defaultRoutingProductChange = defaultChanges.Entity.DefaultRoutingProductChange;

                    int? routingProductId = null;
                    if (newDefaultRoutingProduct != null) routingProductId = newDefaultRoutingProduct.DefaultRoutingProductId;
                    else if (defaultRoutingProductChange != null) routingProductId = defaultRoutingProductChange.DefaultRoutingProductId;

                    if (routingProductId != null)
                        defaultChanges.DefaultRoutingProductName = GetRoutingProductName((int)routingProductId);

                    detailedChanges.DefaultChanges = defaultChanges;
                }

                if (changes.ZoneChanges != null)
                {
                    List<ZoneChangesDetail> lst = new List<ZoneChangesDetail>();

                    foreach (ZoneChanges zoneItemChanges in changes.ZoneChanges)
                    {
                        ZoneChangesDetail itm = new ZoneChangesDetail() { Entity = zoneItemChanges };
                        itm.ZoneName = GetZoneName(zoneItemChanges.ZoneId);

                        int? routingProductId = null;
                        if (zoneItemChanges.NewRoutingProduct != null) routingProductId = zoneItemChanges.NewRoutingProduct.ZoneRoutingProductId;
                        else if (zoneItemChanges.RoutingProductChange != null) routingProductId = zoneItemChanges.RoutingProductChange.ZoneRoutingProductId;

                        if (routingProductId != null)
                            itm.ZoneRoutingProductName = GetRoutingProductName((int)routingProductId);

                        lst.Add(itm);
                    }

                    detailedChanges.ZoneChanges = lst;
                }
            }
            
            return detailedChanges;
        }

        #region Save Changes

        public bool SaveChanges(SaveChangesInput input)
        {
            Changes existingChanges = _dataManager.GetChanges(input.OwnerType, input.OwnerId, RatePlanStatus.Draft);
            Changes allChanges = MergeChanges(existingChanges, input.NewChanges);

            if (allChanges != null)
                return _dataManager.InsertOrUpdateChanges(input.OwnerType, input.OwnerId, allChanges, RatePlanStatus.Draft);
            
            return true;
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
