using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using TOne.WhS.Sales.Entities.RateManagement;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business
{
    public class SaveChangesManager
    {
        SalePriceListOwnerType _ownerType;
        int _ownerId;
        Changes _changes;

        IRatePlanDataManager _ratePlanDataManager;

        public SaveChangesManager(SalePriceListOwnerType ownerType, int ownerId)
        {
            _ownerType = ownerType;
            _ownerId = ownerId;

            _ratePlanDataManager = SalesDataManagerFactory.GetDataManager<IRatePlanDataManager>();
            _changes = _ratePlanDataManager.GetChanges(ownerType, ownerId, RatePlanStatus.Draft);
        }

        #region Save Price List

        public void SavePriceList()
        {
            SaveDefaultChanges();
            SaveRateChanges();
            SaveRoutingProductChanges();
            _ratePlanDataManager.SetRatePlanStatusIfExists(_ownerType, _ownerId, RatePlanStatus.Completed);
        }

        void SaveDefaultChanges()
        {
            if (_changes != null && _changes.DefaultChanges != null)
            {
                ISaleEntityRoutingProductDataManager saleEntityRoutingProductDataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();

                if (_changes.DefaultChanges.NewDefaultRoutingProduct != null)
                    saleEntityRoutingProductDataManager.InsertOrUpdateDefaultRoutingProduct(_ownerType, _ownerId, _changes.DefaultChanges.NewDefaultRoutingProduct);

                else if (_changes.DefaultChanges.DefaultRoutingProductChange != null)
                    saleEntityRoutingProductDataManager.UpdateDefaultRoutingProduct(_ownerType, _ownerId, _changes.DefaultChanges.DefaultRoutingProductChange);
            }
        }

        void SaveRateChanges()
        {
            if (_changes != null && _changes.ZoneChanges != null)
            {
                ISaleRateDataManager saleRateDataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
                IEnumerable<NewRate> newRates = _changes.ZoneChanges.MapRecords(itm => itm.NewRate, itm => itm.NewRate != null);
                List<RateChange> rateChanges = _changes.ZoneChanges.MapRecords(itm => itm.RateChange, itm => itm.RateChange != null).ToList();

                if (newRates != null && newRates.Count() > 0)
                {
                    if (rateChanges == null)
                        rateChanges = new List<RateChange>();

                    PriceListRateManager rateManager = new PriceListRateManager();

                    DateTime minBED = newRates.MapRecords(itm => itm.BED).OrderBy(itm => itm).FirstOrDefault();
                    IEnumerable<long> zoneIds = newRates.MapRecords(itm => itm.ZoneId);
                    IEnumerable<SaleRate> existingRates = saleRateDataManager.GetExistingRatesByZoneIds(_ownerType, _ownerId, zoneIds, minBED);

                    foreach (NewRate newRate in newRates)
                    {
                        IEnumerable<SaleRate> zoneExistingRates = existingRates.FindAllRecords(itm => itm.ZoneId == newRate.ZoneId && itm.IsOverlapedWith(newRate));
                        rateManager.ProcessNewRate(newRate, zoneExistingRates.MapRecords(itm => new ExistingRate() { RateEntity = itm }));
                        rateChanges.AddRange(newRate.ChangedExistingRates.MapRecords(itm => new RateChange() { RateId = itm.ChangedRate.RateId, EED = itm.ChangedRate.EED }));
                    }

                    int priceListId = AddPriceList();
                    saleRateDataManager.InsertRates(newRates, priceListId);
                }

                if (rateChanges != null && rateChanges.Count > 0)
                    saleRateDataManager.CloseRates(rateChanges);
            }
        }

        int AddPriceList()
        {
            int priceListId;

            SalePriceList priceList = new SalePriceList()
            {
                OwnerType = _ownerType,
                OwnerId = _ownerId
            };

            _ratePlanDataManager.InsertPriceList(priceList, out priceListId);

            return priceListId;
        }

        void SaveRoutingProductChanges()
        {
            if (_changes != null && _changes.ZoneChanges != null)
            {
                ISaleEntityRoutingProductDataManager saleEntityRoutingProductDataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();
                var newRoutingProducts = _changes.ZoneChanges.MapRecords(itm => itm.NewRoutingProduct, itm => itm.NewRoutingProduct != null);
                var routingProductChanges = _changes.ZoneChanges.MapRecords(itm => itm.RoutingProductChange, itm => itm.RoutingProductChange != null);

                if (newRoutingProducts != null && newRoutingProducts.Count() > 0)
                    saleEntityRoutingProductDataManager.InsertZoneRoutingProducts(_ownerType, _ownerId, newRoutingProducts);

                if (routingProductChanges != null && routingProductChanges.Count() > 0)
                    saleEntityRoutingProductDataManager.UpdateZoneRoutingProducts(_ownerType, _ownerId, routingProductChanges);
            }
        }

        #endregion

        #region Save Changes

        public void SaveChanges(Changes newChanges)
        {
            Changes allChanges = MergeChanges(_changes, newChanges);

            if (allChanges != null)
                _ratePlanDataManager.InsertOrUpdateChanges(_ownerType, _ownerId, allChanges, RatePlanStatus.Draft);
        }

        Changes MergeChanges(Changes existingChanges, Changes newChanges)
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

        List<ZoneChanges> MergeZoneChanges(List<ZoneChanges> existingZoneChanges, List<ZoneChanges> newZoneChanges)
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

        T Merge<T>(T existingChanges, T newChanges, Func<T> mergeLogic) where T : class
        {
            if (existingChanges != null && newChanges != null)
                return mergeLogic();

            return existingChanges != null ? existingChanges : newChanges;
        }

        #endregion
    }
}
