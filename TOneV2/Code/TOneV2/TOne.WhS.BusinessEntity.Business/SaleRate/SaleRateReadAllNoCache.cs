using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleRateReadAllNoCache : ISaleRateReader
    {

        #region ctor/Local Variables
        SaleRatesByOwner _allSaleRatesByOwner;
        ISaleRateDataManager _saleRateDataManager;
        SalePriceListManager _salePriceListManager;
        #endregion

        #region Public Methods
        public SaleRateReadAllNoCache(IEnumerable<RoutingCustomerInfoDetails> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            _saleRateDataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            _salePriceListManager = new SalePriceListManager();
            _allSaleRatesByOwner = GetAllSaleRatesByOwner(customerInfos, effectiveOn, isEffectiveInFuture);
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
        private SaleRatesByOwner GetAllSaleRatesByOwner(IEnumerable<RoutingCustomerInfoDetails> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            SaleRatesByOwner result = new SaleRatesByOwner();
            SaleRatesByZone saleRateByZone;
            result.SaleRatesByCustomer = new VRDictionary<int, SaleRatesByZone>();
            result.SaleRatesByProduct = new VRDictionary<int, SaleRatesByZone>();

            IEnumerable<SaleRate> saleRates = _saleRateDataManager.GetEffectiveSaleRateByOwner(customerInfos, effectiveOn, isEffectiveInFuture);
            SaleRate tempSaleRate;
            foreach (SaleRate saleRate in saleRates)
            {
                SalePriceList priceList = _salePriceListManager.GetPriceList(saleRate.PriceListId);
                VRDictionary<int, SaleRatesByZone> saleRatesByOwner = priceList.OwnerType == SalePriceListOwnerType.Customer ? result.SaleRatesByCustomer : result.SaleRatesByProduct;

                if (!saleRatesByOwner.TryGetValue(priceList.OwnerId, out saleRateByZone))
                {
                    saleRateByZone = new SaleRatesByZone();
                    saleRatesByOwner.Add(priceList.OwnerId, saleRateByZone);
                }

                SaleRatePriceList saleRatePriceList;

                if (!saleRateByZone.TryGetValue(saleRate.ZoneId, out saleRatePriceList))
                {
                    saleRatePriceList = new SaleRatePriceList();
                    saleRateByZone.Add(saleRate.ZoneId, saleRatePriceList);
                }

                if (saleRate.RateTypeId.HasValue)
                {
                    if (saleRatePriceList.RatesByRateType == null)
                        saleRatePriceList.RatesByRateType = new Dictionary<int, SaleRate>();

                    if (!saleRatePriceList.RatesByRateType.TryGetValue(saleRate.RateTypeId.Value, out tempSaleRate))
                        saleRatePriceList.RatesByRateType.Add(saleRate.RateTypeId.Value, saleRate);
                }
                else
                    saleRatePriceList.Rate = saleRate;
            }
            return result;
        }
        #endregion
    }
}
