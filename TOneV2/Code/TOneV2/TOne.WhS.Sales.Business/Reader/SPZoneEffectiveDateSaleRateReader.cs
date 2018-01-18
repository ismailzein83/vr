using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
namespace TOne.WhS.Sales.Business.Reader
{
    public class SPZoneEffectiveDateSaleRateReader : ISaleRateReader
    {
        #region Fields / Constructors

        private SaleRatesByOwner _allSaleRatesByOwner;
        public SPZoneEffectiveDateSaleRateReader(int sellingProductId, IEnumerable<long> saleZoneIds, DateTime minimumDate, Dictionary<long, DateTime> zoneEffectiveDatesByZoneId)
        {
            _allSaleRatesByOwner = ReadRates(sellingProductId, saleZoneIds, minimumDate, zoneEffectiveDatesByZoneId);
        }

        #endregion

        public SaleRatesByZone GetZoneRates(SalePriceListOwnerType ownerType, int sellingProductId)
        {
            return (ownerType == SalePriceListOwnerType.SellingProduct && _allSaleRatesByOwner != null && _allSaleRatesByOwner.SaleRatesByProduct != null) ? _allSaleRatesByOwner.SaleRatesByProduct.GetRecord(sellingProductId) : null;
        }

        #region Private Methods

        private SaleRatesByOwner ReadRates(int sellingProductId, IEnumerable<long> zoneIds, DateTime minimumDate, Dictionary<long, DateTime> zoneEffectiveDatesByZoneId)
        {
            SaleRatesByOwner saleRatesByOwner = new SaleRatesByOwner
            {
                SaleRatesByCustomer = new VRDictionary<int, SaleRatesByZone>(),
                SaleRatesByProduct = new VRDictionary<int, SaleRatesByZone>()
            };

            SaleRatesByZone saleRateByZone;
            SaleRatePriceList saleRatePriceList;

            IEnumerable<SaleRate> productRates = new SaleRateManager().GetExistingRatesByZoneIds(SalePriceListOwnerType.SellingProduct, sellingProductId, zoneIds, minimumDate);

            if (productRates != null)
            {
                foreach (var productRate in productRates)
                {
                    DateTime zoneEffectiveDate;
                    if (!zoneEffectiveDatesByZoneId.TryGetValue(productRate.ZoneId, out zoneEffectiveDate))
                        continue;
                    if (productRate.IsInTimeRange(zoneEffectiveDate))
                    {
                        VRDictionary<int, SaleRatesByZone> saleRatesByOwnerTemp = saleRatesByOwner.SaleRatesByProduct;
                        saleRateByZone = saleRatesByOwnerTemp.GetOrCreateItem(sellingProductId);
                        saleRatePriceList = saleRateByZone.GetOrCreateItem(productRate.ZoneId, () => { return new SaleRatePriceList() { RatesByRateType = new Dictionary<int, SaleRate>() }; });
                        AddOwnerZoneRate(saleRatePriceList, productRate);
                    }
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
