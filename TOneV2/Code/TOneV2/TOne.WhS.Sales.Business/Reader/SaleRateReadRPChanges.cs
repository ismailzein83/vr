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
        private IEnumerable<SaleRate> _saleRates;
        private int _sellingProductId;
        private int _customerId;
        private List<long> _zoneIds;
        private DateTime _minimumDate;
        private Dictionary<long, DateTime> _soldZonesEffectiveDateTimes;
        SaleRatesByOwner _allSaleRatesByOwner;

        public SaleRateReadRPChanges(IEnumerable<SaleRate> saleRates, RoutingCustomerInfoDetails routingCustomerInfo, List<long> zoneIds, DateTime minimumDateTime, Dictionary<long, DateTime> zonesEffectiveDateTimes)
        {
            _saleRates = saleRates;
            _sellingProductId = routingCustomerInfo.SellingProductId;
            _zoneIds = zoneIds;
            _minimumDate = minimumDateTime;
            _customerId = routingCustomerInfo.CustomerId;
            _soldZonesEffectiveDateTimes = zonesEffectiveDateTimes;
            _allSaleRatesByOwner = GetAllSaleRates();
        }

        public SaleRatesByZone GetZoneRates(SalePriceListOwnerType ownerType, int ownerId)
        {
            if (_allSaleRatesByOwner == null)
                return null;

            var saleRateByOwnerType = ownerType == SalePriceListOwnerType.Customer
                ? _allSaleRatesByOwner.SaleRatesByCustomer
                : _allSaleRatesByOwner.SaleRatesByProduct;

            return saleRateByOwnerType == null ? null : saleRateByOwnerType.GetRecord(ownerId);
        }
        private SaleRatesByOwner GetAllSaleRates()
        {
            SaleRatesByOwner saleRatesByOwner = new SaleRatesByOwner
            {
                SaleRatesByCustomer = new VRDictionary<int, SaleRatesByZone>(),
                SaleRatesByProduct = new VRDictionary<int, SaleRatesByZone>()
            };
            SaleRatesByZone saleRateByZone;
            SaleRatePriceList saleRatePriceList;
            foreach (var saleRate in _saleRates)
            {
                VRDictionary<int, SaleRatesByZone> saleRatesByOwnerTemp = saleRatesByOwner.SaleRatesByCustomer;
                saleRateByZone = saleRatesByOwnerTemp.GetOrCreateItem(_customerId);
                saleRatePriceList = saleRateByZone.GetOrCreateItem(saleRate.ZoneId);
                saleRatePriceList.Rate = saleRate;
            }
            IEnumerable<SaleRate> sellingPRoductSaleRates = GetSellingProductRates();
            foreach (var sellingProductSaleRate in sellingPRoductSaleRates)
            {
                DateTime zoneEffectiveDateTime;
                if (!_soldZonesEffectiveDateTimes.TryGetValue(sellingProductSaleRate.ZoneId, out zoneEffectiveDateTime))
                    continue;
                if (zoneEffectiveDateTime < sellingProductSaleRate.BED ||
                    (sellingProductSaleRate.EED.HasValue && !(sellingProductSaleRate.EED <= zoneEffectiveDateTime)))
                    continue;
                VRDictionary<int, SaleRatesByZone> saleRatesByOwnerTemp = saleRatesByOwner.SaleRatesByProduct;
                saleRateByZone = saleRatesByOwnerTemp.GetOrCreateItem(_customerId);
                saleRatePriceList = saleRateByZone.GetOrCreateItem(sellingProductSaleRate.ZoneId);
                if (sellingProductSaleRate.RateTypeId.HasValue)
                {
                    if (saleRatePriceList.RatesByRateType == null)
                        saleRatePriceList.RatesByRateType = new Dictionary<int, SaleRate>();

                    if (!saleRatePriceList.RatesByRateType.ContainsKey(sellingProductSaleRate.RateTypeId.Value))
                    {
                        saleRatePriceList.RatesByRateType.Add(sellingProductSaleRate.RateTypeId.Value,
                            sellingProductSaleRate);
                    }
                }
                else
                    saleRatePriceList.Rate = sellingProductSaleRate;
            }
            return saleRatesByOwner;
        }
        private IEnumerable<SaleRate> GetSellingProductRates()
        {
            SaleRateManager saleRateManager = new SaleRateManager();
            return saleRateManager.GetExistingRatesByZoneIds(SalePriceListOwnerType.SellingProduct, _sellingProductId, _zoneIds,
                  _minimumDate);
        }
    }
}
