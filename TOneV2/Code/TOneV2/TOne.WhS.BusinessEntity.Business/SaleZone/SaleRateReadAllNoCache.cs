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
        public SaleRateReadAllNoCache(IEnumerable<RoutingCustomerInfo> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            _saleRateDataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
            _salePriceListManager = new SalePriceListManager();
            _allSaleRatesByOwner = GetAllSaleRatesByOwner(customerInfos, effectiveOn, isEffectiveInFuture);
        }
        public SaleRatesByZone GetZoneRates(SalePriceListOwnerType ownerType, int ownerId)
        {
            var saleRateByOwnerType = ownerType == SalePriceListOwnerType.Customer ? _allSaleRatesByOwner.SaleRatesByCustomer : _allSaleRatesByOwner.SaleRatesByProduct;
            SaleRatesByZone saleRatesByZone;
            saleRateByOwnerType.TryGetValue(ownerId, out saleRatesByZone);
            return saleRatesByZone;
        }
        #endregion

        #region Private Members
        private SaleRatesByOwner GetAllSaleRatesByOwner(IEnumerable<RoutingCustomerInfo> customerInfos, DateTime? effectiveOn, bool isEffectiveInFuture)
        {
            SaleRatesByOwner result = new SaleRatesByOwner();
            SaleRatesByZone saleRateByZone;
            result.SaleRatesByCustomer = new Dictionary<int, SaleRatesByZone>();
            result.SaleRatesByProduct = new Dictionary<int, SaleRatesByZone>();

            IEnumerable<SaleRate> saleRates = _saleRateDataManager.GetEffectiveSaleRateByCustomers(customerInfos, effectiveOn, isEffectiveInFuture);

            foreach (SaleRate saleRate in saleRates)
            {
                SaleRatePriceList saleRatePriceList = GetSaleRatePriceList(saleRate);
                Dictionary<int, SaleRatesByZone> saleRatesByOwner = saleRatePriceList.PriceList.OwnerType == SalePriceListOwnerType.Customer ? result.SaleRatesByCustomer : result.SaleRatesByProduct;

                if (!saleRatesByOwner.TryGetValue(saleRatePriceList.PriceList.OwnerId, out saleRateByZone))
                {
                    saleRateByZone = new SaleRatesByZone();
                    saleRatesByOwner.Add(saleRatePriceList.PriceList.OwnerId, saleRateByZone);
                }

                SaleRatePriceList saleRatePriceListItem;
                if (saleRateByZone.TryGetValue(saleRate.ZoneId, out saleRatePriceListItem))
                {
                    if (saleRate.RateTypeId.HasValue)
                    {
                        if (saleRatePriceListItem.RatesByRateType == null)
                            saleRatePriceListItem.RatesByRateType = new Dictionary<int, SaleRate>();
                        saleRatePriceListItem.RatesByRateType.Add(saleRate.RateTypeId.Value, saleRate);
                    }
                    else
                        saleRatePriceListItem.Rate = saleRate;
                }
                else
                {
                    saleRatePriceListItem = saleRatePriceList;
                    if (saleRate.RateTypeId.HasValue)
                    {
                        saleRatePriceListItem.RatesByRateType = new Dictionary<int, SaleRate>();
                        saleRatePriceListItem.RatesByRateType.Add(saleRate.RateTypeId.Value, saleRate);
                    }
                    else
                        saleRatePriceListItem.Rate = saleRate;
                    saleRateByZone.Add(saleRate.ZoneId, saleRatePriceListItem);
                }
            }
            return result;
        }
        private SaleRatePriceList GetSaleRatePriceList(SaleRate saleRate)
        {
            SalePriceList priceList = _salePriceListManager.GetPriceList(saleRate.PriceListId);
            SaleRatePriceList saleRatePriceList = new SaleRatePriceList()
            {
                PriceList = priceList
            };
            return saleRatePriceList;
        }
        #endregion


    }
}
