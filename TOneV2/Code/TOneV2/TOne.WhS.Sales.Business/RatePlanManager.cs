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

                int? sellingProductId = input.Filter.OwnerType == SalePriceListOwnerType.Customer ?
                    GetSellingProductId(input.Filter.OwnerType, input.Filter.OwnerId, DateTime.Now, false) : input.Filter.OwnerId;

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
                        ZoneName = zone.Name
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

        public IEnumerable<TemplateConfig> GetCostCalculationMethodTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.CostCalculationMethod);
        }

        public IEnumerable<TemplateConfig> GetRateCalculationMethodTemplates()
        {
            TemplateConfigManager templateConfigManager = new TemplateConfigManager();
            return templateConfigManager.GetTemplateConfigurations(Constants.RateCalculationMethod);
        }

        public ChangesSummary GetChangesSummary(SalePriceListOwnerType ownerType, int ownerId)
        {
            int? sellingProductId = GetSellingProductId(ownerType, ownerId, DateTime.Now, false);
            GetChangesManager changesManager = new GetChangesManager(ownerType, ownerId, sellingProductId, DateTime.Now);
            return changesManager.GetChangesSummary();
        }

        public Vanrise.Entities.IDataRetrievalResult<ZoneRateChangesDetail> GetFilteredZoneRateChanges(Vanrise.Entities.DataRetrievalInput<ZoneRateChangesQuery> input)
        {
            int? sellingProductId = GetSellingProductId(input.Query.OwnerType, input.Query.OwnerId, DateTime.Now, false);
            GetChangesManager changesManager = new GetChangesManager(input.Query.OwnerType, input.Query.OwnerId, sellingProductId, DateTime.Now);
            IEnumerable<ZoneRateChangesDetail> zoneRateChanges = changesManager.GetFilteredZoneRateChanges();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, zoneRateChanges.ToBigResult(input, null));
        }

        public Vanrise.Entities.IDataRetrievalResult<ZoneRoutingProductChangesDetail> GetFilteredZoneRoutingProductChanges(Vanrise.Entities.DataRetrievalInput<ZoneRoutingProductChangesQuery> input)
        {
            int? sellingProductId = GetSellingProductId(input.Query.OwnerType, input.Query.OwnerId, DateTime.Now, false);
            GetChangesManager changesManager = new GetChangesManager(input.Query.OwnerType, input.Query.OwnerId, sellingProductId, DateTime.Now);
            IEnumerable<ZoneRoutingProductChangesDetail> zoneRoutingProductChanges = changesManager.GetFilteredZoneRoutingProductChanges();
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, zoneRoutingProductChanges.ToBigResult(input, null));
        }

        public void SavePriceList(SalePriceListOwnerType ownerType, int ownerId)
        {
            SaveChangesManager changesManager = new SaveChangesManager(ownerType, ownerId);
            changesManager.SavePriceList();
        }

        public void SaveChanges(SaveChangesInput input)
        {
            SaveChangesManager changesManager = new SaveChangesManager(input.OwnerType, input.OwnerId);
            changesManager.SaveChanges(input.NewChanges);
        }

        public void ApplyCalculatedRates(ApplyCalculatedRatesInput input)
        {
            SaveChangesManager saveChangesManager = new SaveChangesManager(input.OwnerType, input.OwnerId);

            int? sellingNumberPlanId = GetSellingNumberPlanId(input.OwnerType, input.OwnerId);
            int? sellingProductId = GetSellingProductId(input.OwnerType, input.OwnerId, input.EffectiveOn, false);

            saveChangesManager.ApplyCalculatedRates((int)sellingNumberPlanId, (int)sellingProductId, input.EffectiveOn, input.RoutingDatabaseId, input.PolicyConfigId, input.NumberOfOptions, input.CostCalculationMethods, input.SelectedCostCalculationMethodConfigId, input.RateCalculationMethod);
        }

        #region Common Private Methods

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

        #endregion
    }
}
