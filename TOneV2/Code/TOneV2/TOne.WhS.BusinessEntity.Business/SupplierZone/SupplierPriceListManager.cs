using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierPriceListManager
    {

        #region Public Methods
        public SupplierPriceList GetPriceList(int priceListId)
        {
            List<SupplierPriceList> priceLists = GetCachedPriceLists();
            var priceList = priceLists.FindRecord(x => x.PriceListId == priceListId);
            return priceList;
        }

        public Vanrise.Entities.IDataRetrievalResult<SupplierPriceListDetail> GetFilteredSupplierPriceLists(Vanrise.Entities.DataRetrievalInput<SupplierPricelistQuery> input)
        {
            List<SupplierPriceList> allPriceLists = GetCachedPriceLists();
            Func<SupplierPriceList, bool> filterExpression = (item) =>
                (input.Query.SupplierId == null || item.SupplierId == input.Query.SupplierId);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allPriceLists.ToBigResult(input, filterExpression, SupplierPriceListDetailMapper));
        }

        #endregion

        #region Private Members
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISupplierPriceListDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISupplierPriceListDataManager>();
            object _updateHandle;

            public override Vanrise.Caching.CacheObjectSize ApproximateObjectSize
            {
                get
                {
                    return Vanrise.Caching.CacheObjectSize.Large;
                }
            }

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.ArGetPriceListsUpdated(ref _updateHandle);
            }
        }
        List<SupplierPriceList> GetCachedPriceLists()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(String.Format("GetPriceLists"),
               () =>
               {
                   ISupplierPriceListDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierPriceListDataManager>();
                   return dataManager.GetPriceLists();
               });
        }

        private SupplierPriceListDetail SupplierPriceListDetailMapper(SupplierPriceList priceList)
        {
            SupplierPriceListDetail supplierPriceListDetail = new SupplierPriceListDetail();
            supplierPriceListDetail.Entity = priceList;
            CurrencyManager currencyManager = new CurrencyManager();
            Currency currency = currencyManager.GetCurrency(priceList.CurrencyId);
            supplierPriceListDetail.Currency = currency != null ? currency.Name : null;
            CarrierAccountManager carrierAccountManager = new CarrierAccountManager();
            supplierPriceListDetail.SupplierName = carrierAccountManager.GetCarrierAccountName(priceList.SupplierId);

            return supplierPriceListDetail;
        }

        #endregion

    }
}
