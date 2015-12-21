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
        IRatePlanDataManager _dataManager;
        RoutingProductManager _routingProductManager;

        public RatePlanManager()
        {
            _dataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            _routingProductManager = new RoutingProductManager();
        }

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

        public IEnumerable<ZoneItem> GetZoneItems(ZoneItemsInput input)
        {
            int? sellingProductId = GetSellingProductId(input.Filter.OwnerType, input.Filter.OwnerId, DateTime.Now, false);
            int? sellingNumberPlanId = GetSellingNumberPlanId(input.Filter.OwnerType, input.Filter.OwnerId);
            RatePlanZoneManager manager = new RatePlanZoneManager();

            List<ZoneItem> zoneItems = null;
            IEnumerable<SaleZone> zones = manager.GetRatePlanZones(input.Filter.OwnerType, input.Filter.OwnerId, sellingNumberPlanId, DateTime.Now, input.Filter.ZoneLetter, input.FromRow, input.ToRow);

            if (zones != null)
            {
                zoneItems = new List<ZoneItem>();
                Changes changes = _dataManager.GetChanges(input.Filter.OwnerType, input.Filter.OwnerId, RatePlanStatus.Draft);

                ZoneRateSetter rateSetter = new ZoneRateSetter(input.Filter.OwnerType, input.Filter.OwnerId, sellingProductId, DateTime.Now, changes);
                ZoneRoutingProductSetter routingProductSetter = new ZoneRoutingProductSetter(input.Filter.OwnerType, input.Filter.OwnerId, sellingProductId, DateTime.Now, changes);
                
                foreach (SaleZone zone in zones)
                {
                    ZoneItem zoneItem = new ZoneItem()
                    {
                        ZoneId = zone.SaleZoneId,
                        ZoneName = zone.Name
                    };

                    rateSetter.SetZoneRate(zoneItem);
                    routingProductSetter.SetZoneRoutingProduct(zoneItem);

                    zoneItems.Add(zoneItem);
                }

                IEnumerable<RPZone> rpZones = zoneItems.MapRecords(itm => new RPZone() { RoutingProductId = itm.EffectiveRoutingProductId, SaleZoneId = itm.ZoneId });
                ZoneRouteOptionSetter routeOptionSetter = new ZoneRouteOptionSetter(input.Filter.RoutingDatabaseId, input.Filter.PolicyConfigId, input.Filter.NumberOfOptions, rpZones, input.Filter.CostCalculationMethods);
                routeOptionSetter.SetZoneRouteOptionsAndCosts(zoneItems);
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

            ZoneRoutingProductSetter routingProductSetter = new ZoneRoutingProductSetter(input.OwnerType, input.OwnerId, sellingProductId, DateTime.Now, changes);
            routingProductSetter.SetZoneRoutingProduct(zoneItem);

            RPZone rpZone = new RPZone() { RoutingProductId = zoneItem.EffectiveRoutingProductId, SaleZoneId = zoneItem.ZoneId };
            ZoneRouteOptionSetter routeOptionSetter = new ZoneRouteOptionSetter(input.RoutingDatabaseId, input.PolicyConfigId, input.NumberOfOptions, new List<RPZone>() { rpZone }, input.CostCalculationMethods);
            routeOptionSetter.SetZoneRouteOptionsAndCosts(new List<ZoneItem>() { zoneItem });

            return zoneItem;
        }

        #region Private Methods

        int? GetSellingProductId(SalePriceListOwnerType ownerType, int ownerId, DateTime effectiveOn, bool isEffectiveInFuture)
        {
            if (ownerType == SalePriceListOwnerType.Customer)
            {
                CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
                return customerSellingProductManager.GetEffectiveSellingProductId(ownerId, effectiveOn, isEffectiveInFuture); // Review what isEffectiveFuture does
            }
            else // If the owner is a selling product
                return ownerId;
        }

        int? GetSellingNumberPlanId(SalePriceListOwnerType ownerType, int ownerId)
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
        
        #endregion

        #region Remove

        private string GetZoneName(long zoneId)
        {
            SaleZoneManager saleZoneManager = new SaleZoneManager();
            SaleZone saleZone = saleZoneManager.GetSaleZone(zoneId); // I'm assuming that the sale zone manager only gets effective sale zones
            return saleZone != null ? saleZone.Name : null;
        }

        private string GetRoutingProductName(int routingProductId)
        {
            RoutingProductManager routingProductManager = new RoutingProductManager();
            RoutingProduct routingProduct = routingProductManager.GetRoutingProduct(routingProductId);

            return routingProduct != null ? routingProduct.Name : null;
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
        
        #endregion

        public List<TemplateConfig> GetCostCalculationMethodTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CostCalculationMethod);
        }

        public ChangesSummary GetChangesSummary(SalePriceListOwnerType ownerType, int ownerId)
        {
            int? sellingProductId = GetSellingProductId(ownerType, ownerId, DateTime.Now, false);
            ChangesManager changesManager = new ChangesManager(ownerType, ownerId, sellingProductId, DateTime.Now);
            return changesManager.GetChangesSummary();
        }

        public Vanrise.Entities.IDataRetrievalResult<ZoneRateChangesDetail> GetFilteredZoneRateChanges(Vanrise.Entities.DataRetrievalInput<ZoneRateChangesInput> input)
        {
            int? sellingProductId = GetSellingProductId(input.Query.OwnerType, input.Query.OwnerId, DateTime.Now, false);
            ChangesManager changesManager = new ChangesManager(input.Query.OwnerType, input.Query.OwnerId, sellingProductId, DateTime.Now);
            IEnumerable<ZoneRateChangesDetail> zoneRateChanges = changesManager.GetFilteredZoneRateChanges();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, zoneRateChanges.ToBigResult(input, null));
        }

        public Vanrise.Entities.IDataRetrievalResult<ZoneRoutingProductChangesDetail> GetFilteredZoneRoutingProductChanges(Vanrise.Entities.DataRetrievalInput<ZoneRoutingProductChangesInput> input)
        {
            int? sellingProductId = GetSellingProductId(input.Query.OwnerType, input.Query.OwnerId, DateTime.Now, false);
            ChangesManager changesManager = new ChangesManager(input.Query.OwnerType, input.Query.OwnerId, sellingProductId, DateTime.Now);
            IEnumerable<ZoneRoutingProductChangesDetail> zoneRoutingProductChanges = changesManager.GetFilteredZoneRoutingProductChanges();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, zoneRoutingProductChanges.ToBigResult(input, null));
        }
        
        // Remove the call to GetSellingProductId
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

        private void SetDefaultItemChanges(SalePriceListOwnerType ownerType, int ownerId, DefaultItem defaultItem)
        {
            Changes changes = _dataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);

            if (changes != null && changes.DefaultChanges != null)
            {
                NewDefaultRoutingProduct newRoutingProduct = changes.DefaultChanges.NewDefaultRoutingProduct;
                DefaultRoutingProductChange routingProductChange = changes.DefaultChanges.DefaultRoutingProductChange;

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
    }
}
