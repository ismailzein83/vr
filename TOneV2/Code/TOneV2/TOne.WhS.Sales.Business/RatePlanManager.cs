using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities.RatePlanning;
using TOne.WhS.Sales.Entities.RatePlanning.Input;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class RatePlanManager
    {
        private static IRatePlanDataManager _dataManager;

        static RatePlanManager()
        {
            _dataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
        }

        #region Get Zone Letters

        public IEnumerable<char> GetZoneLetters(SalePriceListOwnerType ownerType, int ownerId)
        {
            IEnumerable<char> zoneLetters = null;

            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                IEnumerable<SaleZone> saleZones = GetSellingProductSaleZones(ownerId, DateTime.Now);

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
            IEnumerable<SaleZone> saleZones = GetSaleZones(input.Filter.OwnerType, input.Filter.OwnerId);

            if (saleZones != null)
            {
                saleZones = GetFilteredSaleZones(saleZones, input.Filter);

                if (saleZones != null)
                {
                    saleZones = GetPagedSaleZones(saleZones, input.FromRow, input.ToRow);

                    if (saleZones != null)
                    {
                        IEnumerable<SaleRate> saleRates = GetSaleRates(input.Filter.OwnerType, input.Filter.OwnerId, saleZones, DateTime.Now);
                        zoneItems = BuildZoneItems(saleZones, saleRates);

                        SetChanges(input.Filter.OwnerType, input.Filter.OwnerId, RatePlanStatus.Draft, zoneItems);
                    }
                }
            }

            return zoneItems;
        }

        private IEnumerable<SaleZone> GetSaleZones(SalePriceListOwnerType ownerType, int ownerId)
        {
            IEnumerable<SaleZone> saleZones = null;

            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                saleZones = GetSellingProductSaleZones(ownerId, DateTime.Now);
            }
            else if (ownerType == SalePriceListOwnerType.Customer)
            {
                CustomerZoneManager manager = new CustomerZoneManager();
                saleZones = manager.GetCustomerSaleZones(ownerId, DateTime.Now, false);
            }

            return saleZones;
        }

        private IEnumerable<SaleZone> GetSellingProductSaleZones(int sellingProductId, DateTime effectiveOn)
        {
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            int sellingNumberPlanId = carrierAccountManager.GetSellingNumberPlanId(sellingProductId, CarrierAccountType.Customer);

            SaleZoneManager saleZoneManager = new SaleZoneManager();
            return saleZoneManager.GetSaleZones(sellingNumberPlanId, effectiveOn);
        }

        private IEnumerable<SaleZone> GetFilteredSaleZones(IEnumerable<SaleZone> saleZones, ZoneItemFilter filter)
        {
            return saleZones.FindAllRecords(z => z.Name != null && z.Name.Length > 0 && char.ToLower(z.Name.ElementAt(0)) == char.ToLower(filter.ZoneLetter));
        }

        private IEnumerable<SaleZone> GetPagedSaleZones(IEnumerable<SaleZone> saleZones, int fromRow, int toRow)
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

        private IEnumerable<SaleRate> GetSaleRates(SalePriceListOwnerType ownerType, int ownerId, IEnumerable<SaleZone> saleZones, DateTime effectiveOn)
        {
            List<SaleRate> saleRates = null;
            SaleEntityZoneRateLocator zoneRateLocator = new SaleEntityZoneRateLocator(new SaleRateReadWithCache(effectiveOn));

            if (ownerType == SalePriceListOwnerType.SellingProduct)
            {
                foreach (SaleZone saleZone in saleZones)
                {
                    SaleEntityZoneRate zoneRate = zoneRateLocator.GetSellingProductZoneRate(ownerId, saleZone.SaleZoneId);
                    
                    if (zoneRate != null)
                        saleRates.Add(zoneRate.Rate);
                }
            }
            else if (ownerType == SalePriceListOwnerType.Customer)
            {
                CustomerSellingProductManager customerSellingProductManager = new CustomerSellingProductManager();
                CustomerSellingProduct customerSellingProduct = customerSellingProductManager.GetEffectiveSellingProduct(ownerId, DateTime.Now, false);

                foreach (SaleZone saleZone in saleZones)
                {
                    SaleEntityZoneRate zoneRate = zoneRateLocator.GetCustomerZoneRate(ownerId, customerSellingProduct.SellingProductId, saleZone.SaleZoneId);

                    if (zoneRate != null)
                        saleRates.Add(zoneRate.Rate);
                }
            }

            return saleRates;
        }

        private IEnumerable<ZoneItem> BuildZoneItems(IEnumerable<SaleZone> saleZones, IEnumerable<SaleRate> saleRates)
        {
            List<ZoneItem> zoneItems = new List<ZoneItem>();

            foreach (SaleZone saleZone in saleZones)
            {
                ZoneItem zoneItem = new ZoneItem();

                zoneItem.ZoneId = saleZone.SaleZoneId;
                zoneItem.ZoneName = saleZone.Name;

                SaleRate rate = saleRates.FindRecord(r => r.ZoneId == saleZone.SaleZoneId);

                if (rate != null)
                {
                    zoneItem.CurrentRateId = rate.SaleRateId;
                    zoneItem.CurrentRate = rate.NormalRate;
                    zoneItem.RateBED = rate.BeginEffectiveDate;
                    zoneItem.RateEED = rate.EndEffectiveDate;
                }

                zoneItems.Add(zoneItem);
            }

            return zoneItems;
        }

        #region Reflect Zone Item Changes

        private void SetChanges(SalePriceListOwnerType ownerType, int ownerId, RatePlanStatus status, IEnumerable<ZoneItem> zoneItems)
        {
            Changes existingChanges = _dataManager.GetChanges(ownerType, ownerId, status);

            if (existingChanges != null)
            {
                SetZoneChanges(existingChanges.ZoneChanges, zoneItems);
            }
        }

        private void SetZoneChanges(List<ZoneChanges> existingZoneChanges, IEnumerable<ZoneItem> zoneItems)
        {
            if (existingZoneChanges != null) {
                foreach (ZoneChanges changes in existingZoneChanges)
                {
                    SetZoneRateChanges(changes, zoneItems);
                }
            }
        }

        private void SetZoneRateChanges(ZoneChanges zoneChanges, IEnumerable<ZoneItem> zoneItems)
        {
            if (zoneChanges.NewRate != null)
            {
                ZoneItem zoneItem = zoneItems.FindRecord(o => o.ZoneId == zoneChanges.NewRate.ZoneId);

                if (zoneItem != null)
                {
                    zoneItem.NewRate = zoneChanges.NewRate.NormalRate;
                    zoneItem.RateEED = zoneChanges.NewRate.EED;
                }
            }
            else if (zoneChanges.RateChange != null)
            {
                ZoneItem zoneItem = zoneItems.FindRecord(o => o.CurrentRateId == zoneChanges.RateChange.RateId);

                if (zoneItem != null)
                    zoneItem.RateEED = zoneChanges.RateChange.EED;
            }
        }

        #endregion

        #endregion

        #region Save Price List

        public bool SavePriceList(SalePriceListOwnerType ownerType, int ownerId)
        {
            bool succeeded = false;
            Changes changes = _dataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);

            if (changes != null && changes.ZoneChanges != null)
            {
                int priceListId = AddPriceList(ownerType, ownerId);
                IEnumerable<NewRate> newRates = changes.ZoneChanges.MapRecords(o => o.NewRate, o => o.NewRate != null);
                IEnumerable<SaleRate> newSaleRates = MapNewRatesToSaleRates(newRates, priceListId);
                IEnumerable<RateChange> rateChanges = changes.ZoneChanges.MapRecords(o => o.RateChange, o => o.RateChange != null);

                SaleRateManager saleRateManager = new SaleRateManager();
                succeeded = saleRateManager.CloseRates(rateChanges);
                succeeded = saleRateManager.InsertRates(newSaleRates);
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
