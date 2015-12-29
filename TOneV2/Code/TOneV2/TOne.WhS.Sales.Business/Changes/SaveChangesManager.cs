using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Data;
using TOne.WhS.Sales.Entities;
using TOne.WhS.Sales.Entities.RateManagement;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

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
                OwnerId = _ownerId,
                CurrencyId = GetCurrencyId()
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

        #region Apply Calculated Rates

        public void ApplyCalculatedRates(int sellingNumberPlanId, int sellingProductId, DateTime effectiveOn, int routingDatabaseId, int policyConfigId, int numberOfOptions, List<CostCalculationMethod> costCalculationMethods, int selectedCostCalculationMethodConfigId, RateCalculationMethod rateCalculationMethod)
        {
            IEnumerable<ZoneItem> zoneItems = GetZoneItemsWithCalculatedRate(sellingNumberPlanId, sellingNumberPlanId, effectiveOn, routingDatabaseId, policyConfigId, numberOfOptions, costCalculationMethods, selectedCostCalculationMethodConfigId, rateCalculationMethod);

            if (zoneItems == null)
                return;

            Changes newChanges = GetZoneChanges(zoneItems, effectiveOn);

            if (newChanges == null)
                return;

            SaveChanges(newChanges);
            SavePriceList();
        }

        IEnumerable<ZoneItem> GetZoneItemsWithCalculatedRate(int sellingNumberPlanId, int sellingProductId, DateTime effectiveOn, int routingDatabaseId, int policyConfigId, int numberOfOptions, List<CostCalculationMethod> costCalculationMethods, int selectedCostCalculationMethodConfigId, RateCalculationMethod rateCalculationMethod)
        {
            // Get the sale zones of the owner
            RatePlanZoneManager ratePlanZoneManager = new RatePlanZoneManager();
            IEnumerable<SaleZone> zones = ratePlanZoneManager.GetZones(_ownerType, _ownerId, sellingNumberPlanId, effectiveOn);

            if (zones == null)
                return null;

            // Create a list of zone items, and set the effective routing product for each
            List<ZoneItem> zoneItems = new List<ZoneItem>();
            ZoneRoutingProductSetter routingProductSetter = new ZoneRoutingProductSetter(_ownerType, _ownerId, sellingProductId, effectiveOn, _changes);

            foreach (SaleZone zone in zones)
            {
                ZoneItem zoneItem = new ZoneItem()
                {
                    ZoneId = zone.SaleZoneId,
                    ZoneName = zone.Name
                };

                routingProductSetter.SetZoneRoutingProduct(zoneItem);
                zoneItems.Add(zoneItem);
            }

            // Set the route options, calculate the costs, and calculate the rate for all zone items
            IEnumerable<RPZone> rpZones = zoneItems.MapRecords(itm => new RPZone() { RoutingProductId = itm.EffectiveRoutingProductId, SaleZoneId = itm.ZoneId });
            ZoneRouteOptionSetter routeOptionSetter = new ZoneRouteOptionSetter(routingDatabaseId, policyConfigId, numberOfOptions, rpZones, costCalculationMethods, selectedCostCalculationMethodConfigId, rateCalculationMethod);

            routeOptionSetter.SetZoneRouteOptionProperties(zoneItems);
            return zoneItems.FindAllRecords(itm => itm.CalculatedRate != null);
        }

        Changes GetZoneChanges(IEnumerable<ZoneItem> zoneItems, DateTime effectiveOn)
        {
            // Create a list of zone changes, each having a new rate, from the calculated rates
            List<ZoneChanges> zoneChanges = new List<ZoneChanges>();
            int currencyId = GetCurrencyId();

            foreach (ZoneItem zoneItem in zoneItems)
            {
                NewRate newRate = new NewRate()
                {
                    ZoneId = zoneItem.ZoneId,
                    NormalRate = (decimal)zoneItem.CalculatedRate,
                    BED = effectiveOn,
                    CurrencyId = currencyId
                };

                ZoneChanges zoneItemChanges = new ZoneChanges() { NewRate = newRate };
                zoneChanges.Add(zoneItemChanges);
            }

            return new Changes() { ZoneChanges = zoneChanges };
        }

        #endregion

        // This method should be moved to CurrencyManager. This is just a quick fix
        int GetCurrencyId()
        {
            CurrencyManager currencyManager = new CurrencyManager();
            IEnumerable<Currency> currencies = currencyManager.GetAllCurrencies();
            string currencyName = System.Configuration.ConfigurationManager.AppSettings["Currency"];

            Currency currency = currencies.FindRecord(itm => itm.Name == currencyName);

            if (currency != null)
                return currency.CurrencyId;
            else
                throw new Exception("The system doesn't have a default currency");
        }
    }
}
