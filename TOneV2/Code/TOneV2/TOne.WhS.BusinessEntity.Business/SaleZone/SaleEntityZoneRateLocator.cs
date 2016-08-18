using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityZoneRateLocator
    {
        #region Fields

        ISaleRateReader _reader;
        SalePriceListManager _salePriceListManager;
        
        #endregion

        #region Public Methods

        public SaleEntityZoneRateLocator(ISaleRateReader reader)
        {
            _reader = reader;
            _salePriceListManager = new SalePriceListManager();
        }
        public SaleEntityZoneRate GetCustomerZoneRate(int customerId, int sellingProductId, long saleZoneId)
        {
            var mergedZoneRate = new SaleEntityZoneRate();

            SaleEntityZoneRate customerZoneRate = GetZoneRate(SalePriceListOwnerType.Customer, customerId, saleZoneId);
            SaleEntityZoneRate sellingProductZoneRate = GetZoneRate(SalePriceListOwnerType.SellingProduct, sellingProductId, saleZoneId);

            MergeZoneRates(customerZoneRate, sellingProductZoneRate, out mergedZoneRate);

            return mergedZoneRate;
        }
        public SaleEntityZoneRate GetSellingProductZoneRate(int sellingProductId, long saleZoneId)
        {
            return GetZoneRate(SalePriceListOwnerType.SellingProduct, sellingProductId, saleZoneId);
        }
        
        #endregion

        #region Private Members

        private SaleEntityZoneRate GetZoneRate(SalePriceListOwnerType ownerType, int ownerId, long saleZoneId)
        {
            SaleRatesByZone zoneRates = _reader.GetZoneRates(ownerType, ownerId);
            SaleRatePriceList saleRatePriceList;
            if (zoneRates != null && zoneRates.TryGetValue(saleZoneId, out saleRatePriceList))
            {
                return new SaleEntityZoneRate
                {
                    Source = ownerType,
                    Rate = saleRatePriceList.Rate,
                    RatesByRateType = saleRatePriceList.RatesByRateType
                };
            }
            return null;
        }
        private void MergeZoneRates(SaleEntityZoneRate customerZoneRate, SaleEntityZoneRate sellingProductZoneRate, out SaleEntityZoneRate mergedZoneRate)
        {
            if (customerZoneRate != null && sellingProductZoneRate != null)
            {
                var zoneRate = new SaleEntityZoneRate();
                
                zoneRate.Rate = (customerZoneRate.Rate != null) ? customerZoneRate.Rate : sellingProductZoneRate.Rate;

                IEnumerable<SaleRate> customerOtherRates = new List<SaleRate>();
                IEnumerable<int> customerRateTypeIds = new List<int>();

                if (customerZoneRate.RatesByRateType != null)
                {
                    customerOtherRates = customerZoneRate.RatesByRateType.Values;
                    customerRateTypeIds = customerZoneRate.RatesByRateType.MapRecords(x => x.Key);
                }

                IEnumerable<SaleRate> sellingProductOtherRates = new List<SaleRate>();
                if (sellingProductZoneRate.RatesByRateType != null)
                    sellingProductOtherRates = sellingProductZoneRate.RatesByRateType.Values;

                var zoneOtherRates = customerOtherRates.Union(sellingProductOtherRates.FindAllRecords(x => !customerRateTypeIds.Contains(x.RateTypeId.Value)));
                zoneRate.RatesByRateType = zoneOtherRates.ToDictionary(x => x.RateTypeId.Value);

                mergedZoneRate = zoneRate;
            }
            else if (customerZoneRate != null)
                mergedZoneRate = customerZoneRate;
            else if (sellingProductZoneRate != null)
                mergedZoneRate = sellingProductZoneRate;
            else
                mergedZoneRate = null;
        }

        #endregion
    }
}
