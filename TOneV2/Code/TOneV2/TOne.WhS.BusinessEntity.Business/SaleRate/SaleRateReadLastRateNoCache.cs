using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleRateReadLastRateNoCache: ISaleRateReader
    {
         #region ctor/Local Variables
        SaleRatesByOwner _allSaleRatesByOwner;
        ISaleRateDataManager _saleRateDataManager;
        SalePriceListManager _salePriceListManager;
        #endregion

        #region Public Methods
        public SaleRateReadLastRateNoCache(IEnumerable<RoutingCustomerInfoDetails> customerInfos, DateTime effectiveAfter)
        {
            _saleRateDataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            _salePriceListManager = new SalePriceListManager();
            _allSaleRatesByOwner = GetAllSaleRatesByOwner(customerInfos, effectiveAfter);
        }
        public SaleRatesByZone GetZoneRates(SalePriceListOwnerType ownerType, int ownerId)
        {
            if (_allSaleRatesByOwner == null)
                return null;

            var saleRateByOwnerType = ownerType == SalePriceListOwnerType.Customer ? _allSaleRatesByOwner.SaleRatesByCustomer : _allSaleRatesByOwner.SaleRatesByProduct;

            if (saleRateByOwnerType == null)
                return null;

            return saleRateByOwnerType.GetRecord(ownerId);
        }
        #endregion

        #region Private Members
        private SaleRatesByOwner GetAllSaleRatesByOwner(IEnumerable<RoutingCustomerInfoDetails> customerInfos, DateTime effectiveAfter)
        {
            SaleRatesByOwner result = new SaleRatesByOwner();
            SaleRatesByZone saleRateByZone;
            SaleRatePriceList saleRatePriceList;

            result.SaleRatesByCustomer = new VRDictionary<int, SaleRatesByZone>();
            result.SaleRatesByProduct = new VRDictionary<int, SaleRatesByZone>();

            IEnumerable<SaleRate> saleRates = _saleRateDataManager.GetEffectiveAfterByMultipleOwners(customerInfos, effectiveAfter);

            foreach (SaleRate saleRate in saleRates)
            {
                SalePriceList priceList = _salePriceListManager.GetPriceList(saleRate.PriceListId);
                VRDictionary<int, SaleRatesByZone> saleRatesByOwner = priceList.OwnerType == SalePriceListOwnerType.Customer ? result.SaleRatesByCustomer : result.SaleRatesByProduct;

                saleRateByZone = saleRatesByOwner.GetOrCreateItem(priceList.OwnerId);
                saleRatePriceList = saleRateByZone.GetOrCreateItem(saleRate.ZoneId);

                if (saleRate.RateTypeId.HasValue)
                {
                    if (saleRatePriceList.RatesByRateType == null)
                        saleRatePriceList.RatesByRateType = new Dictionary<int, SaleRate>();

                    SaleRate existingRate;
                    if (saleRatePriceList.RatesByRateType.TryGetValue(saleRate.RateTypeId.Value, out existingRate))
                    {
                        if (existingRate.BED < saleRate.BED)
                            saleRatePriceList.RatesByRateType[saleRate.RateTypeId.Value] = saleRate;
                    }
                    else
                    {
                        saleRatePriceList.RatesByRateType.Add(saleRate.RateTypeId.Value, saleRate);
                    }
                }
                else if (saleRatePriceList.Rate == null || saleRatePriceList.Rate.BED < saleRate.BED)
                {
                    saleRatePriceList.Rate = saleRate;
                }
            }

            //remove customer rates with EED less then product rates EED
            foreach(var customerSaleRatesByZoneEntry in result.SaleRatesByCustomer)
            {
                int customerId = customerSaleRatesByZoneEntry.Key;
                int productId = customerInfos.FindRecord(itm => itm.CustomerId == customerId).SellingProductId;
                SaleRatesByZone productRatesByZone;
                if(result.SaleRatesByProduct.TryGetValue(productId, out productRatesByZone))
                {
                    List<long> zoneIdsToRemove = new List<long>();
                    var customerRatesByZone = customerSaleRatesByZoneEntry.Value;
                    foreach (var customerRatesEntry in customerRatesByZone)
                    {
                        long zoneId = customerRatesEntry.Key;
                        
                        SaleRatePriceList productRates;
                        if (productRatesByZone.TryGetValue(zoneId, out productRates))
                        {
                            SaleRatePriceList customerRates = customerRatesEntry.Value;
                            if(customerRates.Rate != null && productRates.Rate != null && productRates.Rate.EED.VRGreaterThan(customerRates.Rate.EED))
                            {
                                customerRates.Rate = null;
                            }
                            if(customerRates.RatesByRateType != null && productRates.RatesByRateType != null)
                            {
                                List<int> otherRateTypeIdsToRemove = new List<int>();
                                foreach(var customerOtherRateEntry in customerRates.RatesByRateType)
                                {
                                    int rateTypeId = customerOtherRateEntry.Key;
                                    SaleRate productOtherRate;
                                    if(productRates.RatesByRateType.TryGetValue(rateTypeId, out productOtherRate))
                                    {
                                        SaleRate customerOtherRate = customerOtherRateEntry.Value;
                                        if (productOtherRate.EED.VRGreaterThan(customerOtherRate.EED))
                                            otherRateTypeIdsToRemove.Add(rateTypeId);
                                    }
                                }
                                if(otherRateTypeIdsToRemove.Count > 0)
                                    otherRateTypeIdsToRemove.ForEach(rateTypeId => customerRates.RatesByRateType.Remove(rateTypeId));
                            }
                            if (customerRates.Rate == null && (customerRates.RatesByRateType == null || customerRates.RatesByRateType.Count == 0))
                                zoneIdsToRemove.Add(zoneId);
                        }
                    }
                    if(zoneIdsToRemove.Count > 0)
                        zoneIdsToRemove.ForEach(zoneId => customerRatesByZone.Remove(zoneId));
                }
            }

            return result;
        }

        #endregion
    }
}
