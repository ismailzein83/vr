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
        private SaleRatesByOwner _allSaleRatesByOwner;

        public SaleRateReadRPChanges(IEnumerable<SaleRate> customerSaleRates, RoutingCustomerInfoDetails routingCustomerInfo, List<long> zoneIds,
            DateTime minimumDate, Dictionary<long, DateTime> zonesEffectiveDateTimes)
        {
            //TODO: consider for this type of readers that selling product may be different in past
            _allSaleRatesByOwner = GetAllSaleRates(customerSaleRates, routingCustomerInfo.SellingProductId,
                routingCustomerInfo.CustomerId, zoneIds, minimumDate, zonesEffectiveDateTimes);
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
                saleRatePriceList = saleRateByZone.GetOrCreateItem(saleRate.ZoneId);
                saleRatePriceList.Rate = saleRate;
            }

            SaleRateManager saleRateManager = new SaleRateManager();
            IEnumerable<SaleRate> sellingProductSaleRates = saleRateManager.GetExistingRatesByZoneIds(SalePriceListOwnerType.SellingProduct, sellingProductId,
                zoneIds, minimumDate);

            foreach (var sellingProductSaleRate in sellingProductSaleRates)
            {
                DateTime zoneEffectiveDateTime;
                if (!zonesEffectiveDateTimes.TryGetValue(sellingProductSaleRate.ZoneId, out zoneEffectiveDateTime))
                    continue;
                if (sellingProductSaleRate.IsInTimeRange(zoneEffectiveDateTime) && sellingProductSaleRate.RateTypeId == null)
                {
                    VRDictionary<int, SaleRatesByZone> saleRatesByOwnerTemp = saleRatesByOwner.SaleRatesByProduct;
                    saleRateByZone = saleRatesByOwnerTemp.GetOrCreateItem(sellingProductId);
                    saleRatePriceList = saleRateByZone.GetOrCreateItem(sellingProductSaleRate.ZoneId);
                    saleRatePriceList.Rate = sellingProductSaleRate;
                }
            }
            return saleRatesByOwner;
        }
    }
}
