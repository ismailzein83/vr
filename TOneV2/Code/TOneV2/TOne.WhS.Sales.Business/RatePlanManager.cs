using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using TOne.WhS.Sales.Entities.RatePlanning;
using TOne.WhS.Sales.Entities.RatePlanning.Input;
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
            IEnumerable<ZoneItem> zoneItems = null;
            IEnumerable<SaleZone> zones = GetZones(input.Filter.OwnerType, input.Filter.OwnerId);

            if (zones != null)
            {
                zones = GetFilteredZones(zones, input.Filter);

                if (zones != null)
                {
                    zones = GetPagedZones(zones, input.FromRow, input.ToRow);

                    if (zones != null)
                    {
                        IEnumerable<SaleEntityZoneRate> zoneRates = GetZoneRates(input.Filter.OwnerType, input.Filter.OwnerId, zones, DateTime.Now);
                        zoneItems = BuildZoneItems(input.Filter.OwnerType, zones, zoneRates);

                        SetZoneItemChanges(input.Filter.OwnerType, input.Filter.OwnerId, zoneItems);
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

        private IEnumerable<SaleEntityZoneRate> GetZoneRates(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<SaleZone> zones, DateTime effectiveOn)
        {
            List<SaleEntityZoneRate> zoneRates = null;
            SaleEntityZoneRateLocator rateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(effectiveOn));

            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                zoneRates = new List<SaleEntityZoneRate>();

                foreach (SaleZone saleZone in zones)
                {
                    SaleEntityZoneRate zoneRate = rateLocator.GetSellingProductZoneRate(ownerId, saleZone.SaleZoneId);
                    
                    if (zoneRate != null)
                        zoneRates.Add(zoneRate);
                }
            }
            else if (ownerType == SalePriceListOwnerType.Customer)
            {
                zoneRates = new List<SaleEntityZoneRate>();

                foreach (SaleZone saleZone in zones)
                {
                    SaleEntityZoneRate zoneRate = rateLocator.GetCustomerZoneRate(ownerId, GetSellingProductId(ownerId), saleZone.SaleZoneId);

                    if (zoneRate != null)
                        zoneRates.Add(zoneRate);
                }
            }

            return zoneRates;
        }

        private int GetSellingProductId(int customerId)
        {
            CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
            CustomerSellingProduct customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(customerId, DateTime.Now, false);
            return customerSellingProduct.SellingProductId;
        }

        private IEnumerable<ZoneItem> BuildZoneItems(SalePriceListOwnerType ownerType, IEnumerable<SaleZone> zones, IEnumerable<SaleEntityZoneRate> zoneRates)
        {
            List<ZoneItem> zoneItems = new List<ZoneItem>();

            foreach (SaleZone zone in zones)
            {
                ZoneItem zoneItem = new ZoneItem();

                zoneItem.ZoneId = zone.SaleZoneId;
                zoneItem.ZoneName = zone.Name;

                SaleEntityZoneRate zoneRate = zoneRates.FindRecord(o => o.Rate.ZoneId == zone.SaleZoneId);

                if (zoneRate != null)
                {
                    zoneItem.CurrentRateId = zoneRate.Rate.SaleRateId;
                    zoneItem.CurrentRate = zoneRate.Rate.NormalRate;
                    zoneItem.IsCurrentRateEditable = (zoneRate.Source == ownerType);
                    zoneItem.RateBED = zoneRate.Rate.BeginEffectiveDate;
                    zoneItem.RateEED = zoneRate.Rate.EndEffectiveDate;
                }

                zoneItems.Add(zoneItem);
            }

            return zoneItems;
        }

        #region Set Zone Item Changes

        private void SetZoneItemChanges(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<ZoneItem> zoneItems)
        {
            Changes existingChanges = _dataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);

            if (existingChanges != null && existingChanges.ZoneChanges != null)
            {
                foreach (ZoneChanges zoneItemChanges in existingChanges.ZoneChanges)
                {
                    SetZoneItemRateChanges(zoneItemChanges.NewRate, zoneItemChanges.RateChange, zoneItems);
                    //SetZoneRoutingProductChanges
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
                    zoneItem.RateBED = newRate.BED;
                    zoneItem.RateEED = newRate.EED;
                }
            }
            else if (rateChange != null)
            {
                ZoneItem zoneItem = zoneItems.FindRecord(item => item.CurrentRateId == rateChange.RateId);

                if (zoneItem != null)
                    zoneItem.RateEED = rateChange.EED;
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

            defaultItem.IsCurrentDefaultRoutingProductEditable = (
                (locatorResult.Source == SaleEntityZoneRoutingProductSource.ProductDefault && ownerType == SalePriceListOwnerType.SellingProduct) ||
                (locatorResult.Source == SaleEntityZoneRoutingProductSource.CustomerDefault && ownerType == SalePriceListOwnerType.Customer)
            );

            defaultItem.CurrentDefaultRoutingProductId = locatorResult.RoutingProductId;
            defaultItem.CurrentBED = locatorResult.BED;
            defaultItem.CurrentEED = locatorResult.EED;

            SetDefaultItemChanges(ownerType, ownerId, defaultItem);

            return defaultItem;
        }

        public void SetDefaultItemChanges(SalePriceListOwnerType ownerType, int ownerId, DefaultItem defaultItem)
        {
            Changes existingChanges = _dataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);

            if (existingChanges != null && existingChanges.DefaultChanges != null)
            {
                NewDefaultRoutingProduct newRoutingProduct = existingChanges.DefaultChanges.NewDefaultRoutingProduct;
                DefaultRoutingProductChange routingProductChange = existingChanges.DefaultChanges.DefaultRoutingProductChange;

                if (newRoutingProduct != null)
                {
                    defaultItem.NewDefaultRoutingProductId = newRoutingProduct.DefaultRoutingProductId;
                    defaultItem.CurrentBED = newRoutingProduct.BED;
                    defaultItem.CurrentEED = newRoutingProduct.EED;
                }
                else
                    defaultItem.CurrentEED = routingProductChange.EED;
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
                    
                    IRoutingProductDataManager routingProductDataManager = BEDataManagerFactory.GetDataManager<IRoutingProductDataManager>();

                    if (newDefaultRoutingProduct != null)
                        routingProductDataManager.InsertOrUpdateDefaultRoutingProduct(ownerType, ownerId, newDefaultRoutingProduct);
                    else
                        routingProductDataManager.UpdateDefaultRoutingProduct(ownerType, ownerId, defaultRoutingProductChange);
                }

                if (changes.ZoneChanges != null)
                {
                    int priceListId = AddPriceList(ownerType, ownerId);

                    IEnumerable<NewRate> newRates = changes.ZoneChanges.MapRecords(item => item.NewRate, item => item != null);
                    IEnumerable<SaleRate> newSaleRates = MapNewRatesToSaleRates(newRates, priceListId);
                    IEnumerable<RateChange> rateChanges = changes.ZoneChanges.MapRecords(item => item.RateChange, item => item != null);

                    SaleRateManager saleRateManager = new SaleRateManager();
                    succeeded = saleRateManager.CloseRates(rateChanges);
                    succeeded = saleRateManager.InsertRates(newSaleRates);
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
            bool saved = true;

            if (input.NewChanges != null)
            {
                Changes existingChanges = _dataManager.GetChanges(input.OwnerType, input.OwnerId, RatePlanStatus.Draft);
                Changes allChanges = MergeChanges(existingChanges, input.NewChanges);

                saved = _dataManager.InsertOrUpdateChanges(input.OwnerType, input.OwnerId, allChanges, RatePlanStatus.Draft);
            }

            return saved;
        }

        private Changes MergeChanges(Changes existingChanges, Changes newChanges)
        {
            return Merge(existingChanges, newChanges,
                () =>
                {
                    Changes allChanges = new Changes();
                    allChanges.ZoneChanges = MergeZoneChanges(existingChanges.ZoneChanges, newChanges.ZoneChanges);
                    allChanges.DefaultChanges = newChanges.DefaultChanges;
                    return allChanges;
                });
        }

        private List<ZoneChanges> MergeZoneChanges(List<ZoneChanges> existingZoneChanges, List<ZoneChanges> newZoneChanges)
        {
            return Merge(existingZoneChanges, newZoneChanges,
                () =>
                {
                    foreach (ZoneChanges changes in existingZoneChanges)
                    {
                        if (!newZoneChanges.Any(o => o.ZoneId == changes.ZoneId))
                            newZoneChanges.Add(changes);
                    }

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
