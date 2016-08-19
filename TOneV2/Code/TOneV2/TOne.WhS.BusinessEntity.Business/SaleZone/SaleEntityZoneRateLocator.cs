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
            SaleEntityZoneRate customerZoneRate = GetZoneRate(SalePriceListOwnerType.Customer, customerId, saleZoneId);
            SaleEntityZoneRate sellingProductZoneRate = GetZoneRate(SalePriceListOwnerType.SellingProduct, sellingProductId, saleZoneId);
            return GetMergedZoneRate(customerZoneRate, sellingProductZoneRate);
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
                var zoneRate = new SaleEntityZoneRate
                {
                    Source = ownerType,
                    Rate = saleRatePriceList.Rate,
                    RatesByRateType = saleRatePriceList.RatesByRateType
                };

                zoneRate.SourcesByRateType = new Dictionary<int, SalePriceListOwnerType>();
                if (zoneRate.RatesByRateType != null)
                {
                    foreach (KeyValuePair<int, SaleRate> kvp in zoneRate.RatesByRateType)
                        zoneRate.SourcesByRateType.Add(kvp.Key, ownerType);
                }

                return zoneRate;
            }
            return null;
        }
        private SaleEntityZoneRate GetMergedZoneRate(SaleEntityZoneRate customerZoneRate, SaleEntityZoneRate sellingProductZoneRate)
        {
            SaleEntityZoneRate mergedZoneRate;

            if (customerZoneRate != null && sellingProductZoneRate != null)
            {
                var zoneRate = new SaleEntityZoneRate();

                if (customerZoneRate.Rate != null)
                {
                    zoneRate.Rate = customerZoneRate.Rate;
                    zoneRate.Source = customerZoneRate.Source;
                }
                else
                {
                    zoneRate.Rate = sellingProductZoneRate.Rate;
                    zoneRate.Source = sellingProductZoneRate.Source;
                }

                zoneRate.SourcesByRateType = new Dictionary<int, SalePriceListOwnerType>();

                IEnumerable<SaleRate> customerOtherRates = new List<SaleRate>();
                IEnumerable<int> customerRateTypeIds = new List<int>();

                if (customerZoneRate.RatesByRateType != null)
                {
                    customerOtherRates = customerZoneRate.RatesByRateType.Values;
                    customerRateTypeIds = customerZoneRate.RatesByRateType.Keys;
                    foreach (int customerRateTypeId in customerRateTypeIds)
                        zoneRate.SourcesByRateType.Add(customerRateTypeId, customerZoneRate.Source);
                }

                IEnumerable<SaleRate> sellingProductOtherRates = new List<SaleRate>();
                if (sellingProductZoneRate.RatesByRateType != null)
                {
                    sellingProductOtherRates = sellingProductZoneRate.RatesByRateType.Values.FindAllRecords(x => !customerRateTypeIds.Contains(x.RateTypeId.Value));
                    foreach (SaleRate sellingProductOtherRate in sellingProductOtherRates)
                        zoneRate.SourcesByRateType.Add(sellingProductOtherRate.RateTypeId.Value, sellingProductZoneRate.Source);
                }

                var zoneOtherRates = customerOtherRates.Union(sellingProductOtherRates);
                zoneRate.RatesByRateType = zoneOtherRates.ToDictionary(x => x.RateTypeId.Value);

                mergedZoneRate = zoneRate;
            }
            else if (customerZoneRate != null)
                mergedZoneRate = customerZoneRate;
            else if (sellingProductZoneRate != null)
                mergedZoneRate = sellingProductZoneRate;
            else
                mergedZoneRate = null;

            return mergedZoneRate;
        }

        #endregion
    }
}
