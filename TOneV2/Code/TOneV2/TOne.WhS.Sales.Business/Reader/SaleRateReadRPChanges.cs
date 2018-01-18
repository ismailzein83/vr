using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.Sales.Business.Reader
{
    public class SaleRateReadRPChanges : ISaleRateReader
    {
        #region Fields / Constructors

        private SaleRatesByOwner _allSaleRatesByOwner;

        public SaleRateReadRPChanges(IEnumerable<SaleRate> customerSaleRates, RoutingCustomerInfoDetails routingCustomerInfo, List<long> zoneIds, DateTime minimumDate, Dictionary<long, DateTime> zonesEffectiveDateTimes)
        {
            _allSaleRatesByOwner = GetAllSaleRates(customerSaleRates, routingCustomerInfo.SellingProductId, routingCustomerInfo.CustomerId, zoneIds, minimumDate, zonesEffectiveDateTimes);
        }
        public SaleRateReadRPChanges(int customerId, int sellingProductId, IEnumerable<long> saleZoneIds, DateTime minimumDate, Dictionary<long, DateTime> zoneEffectiveDatesByZoneIds)
        {
            IEnumerable<SaleRate> customerZoneRates = new SaleRateManager().GetExistingRatesByZoneIds(SalePriceListOwnerType.Customer, customerId, saleZoneIds, minimumDate);

            var validCustomerZoneRates = new List<SaleRate>();
            foreach (SaleRate customerZoneRate in customerZoneRates)
            {
                DateTime zoneEffectiveDate;
                if (!zoneEffectiveDatesByZoneIds.TryGetValue(customerZoneRate.ZoneId, out zoneEffectiveDate))
                    throw new Vanrise.Entities.DataIntegrityValidationException(string.Format("The action date of zone '{0}' was not found", customerZoneRate.ZoneId));

                if (customerZoneRate.IsInTimeRange(zoneEffectiveDate))
                    validCustomerZoneRates.Add(customerZoneRate);
            }

            _allSaleRatesByOwner = GetAllSaleRates(validCustomerZoneRates, sellingProductId, customerId, saleZoneIds, minimumDate, zoneEffectiveDatesByZoneIds);
        }

        #endregion

        public SaleRatesByZone GetZoneRates(SalePriceListOwnerType ownerType, int ownerId)
        {
            if (_allSaleRatesByOwner == null)
                return null;

            var saleRateByOwnerType = ownerType == SalePriceListOwnerType.Customer
                ? _allSaleRatesByOwner.SaleRatesByCustomer
                : _allSaleRatesByOwner.SaleRatesByProduct;

            return saleRateByOwnerType == null ? null : saleRateByOwnerType.GetRecord(ownerId);
        }

        #region Private Methods

        private SaleRatesByOwner GetAllSaleRates(IEnumerable<SaleRate> customerSaleRates, int sellingProductId, int customerId, IEnumerable<long> zoneIds, DateTime minimumDate, Dictionary<long, DateTime> zonesEffectiveDateTimes)
        {
            SaleRatesByOwner saleRatesByOwner = new SaleRatesByOwner
            {
                SaleRatesByCustomer = new VRDictionary<int, SaleRatesByZone>(),
                SaleRatesByProduct = new VRDictionary<int, SaleRatesByZone>()
            };
            SaleRatesByZone saleRateByZone;
            SaleRatePriceList saleRatePriceList;
            foreach (var saleRate in customerSaleRates)
            {
                VRDictionary<int, SaleRatesByZone> saleRatesByOwnerTemp = saleRatesByOwner.SaleRatesByCustomer;
                saleRateByZone = saleRatesByOwnerTemp.GetOrCreateItem(customerId);
                saleRatePriceList = saleRateByZone.GetOrCreateItem(saleRate.ZoneId, () => { return new SaleRatePriceList() { RatesByRateType = new Dictionary<int, SaleRate>() }; });
                AddOwnerZoneRate(saleRatePriceList, saleRate);
            }

            SaleRateManager saleRateManager = new SaleRateManager();
            IEnumerable<SaleRate> sellingProductSaleRates = saleRateManager.GetExistingRatesByZoneIds(SalePriceListOwnerType.SellingProduct, sellingProductId,
                zoneIds, minimumDate);

            foreach (var sellingProductSaleRate in sellingProductSaleRates)
            {
                DateTime zoneEffectiveDateTime;
                if (!zonesEffectiveDateTimes.TryGetValue(sellingProductSaleRate.ZoneId, out zoneEffectiveDateTime))
                    continue;
                if (sellingProductSaleRate.IsInTimeRange(zoneEffectiveDateTime))
                {
                    VRDictionary<int, SaleRatesByZone> saleRatesByOwnerTemp = saleRatesByOwner.SaleRatesByProduct;
                    saleRateByZone = saleRatesByOwnerTemp.GetOrCreateItem(sellingProductId);
                    saleRatePriceList = saleRateByZone.GetOrCreateItem(sellingProductSaleRate.ZoneId, () => { return new SaleRatePriceList() { RatesByRateType = new Dictionary<int, SaleRate>() }; });
                    AddOwnerZoneRate(saleRatePriceList, sellingProductSaleRate);
                }
            }
            return saleRatesByOwner;
        }
        private void AddOwnerZoneRate(SaleRatePriceList ownerZoneRates, SaleRate rate)
        {
            if (!rate.RateTypeId.HasValue)
                ownerZoneRates.Rate = rate;
            else
            {
                if (!ownerZoneRates.RatesByRateType.ContainsKey(rate.RateTypeId.Value))
                    ownerZoneRates.RatesByRateType.Add(rate.RateTypeId.Value, rate);
            }
        }

        #endregion
    }
}
