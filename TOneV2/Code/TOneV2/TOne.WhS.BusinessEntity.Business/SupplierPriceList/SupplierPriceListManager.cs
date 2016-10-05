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
            Dictionary<int, SupplierPriceList> priceLists = GetCachedPriceLists();
            var priceList = priceLists.GetRecord(priceListId);
            return priceList;
        }

        public Vanrise.Entities.IDataRetrievalResult<SupplierPriceListDetail> GetFilteredSupplierPriceLists(Vanrise.Entities.DataRetrievalInput<SupplierPricelistQuery> input)
        {
            Dictionary<int, SupplierPriceList> allPriceLists = GetCachedPriceLists();
            Func<SupplierPriceList, bool> filterExpression = (item) =>
                (input.Query.SupplierIds == null || input.Query.SupplierIds.Contains(item.SupplierId))
                && (input.Query.FromDate == null || item.CreateTime >= input.Query.FromDate)
                && (!input.Query.ToDate.HasValue || item.CreateTime <= input.Query.ToDate);
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allPriceLists.ToBigResult(input, filterExpression, SupplierPriceListDetailMapper));
        }

        public long ReserveIDRange(int numberOfIDs)
        {
            long startingId;
            IDManager.Instance.ReserveIDRange(GetSupplierPriceListType(), numberOfIDs, out startingId);
            return startingId;
        }


        public int GetSupplierPriceListTypeId()
        {
            return Vanrise.Common.Business.TypeManager.Instance.GetTypeId(this.GetSupplierPriceListType());
        }

        public Type GetSupplierPriceListType()
        {
            return this.GetType();
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
        public Dictionary<int, SupplierPriceList> GetCachedPriceLists()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(String.Format("GetPriceLists"),
               () =>
               {
                   ISupplierPriceListDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierPriceListDataManager>();
                   IEnumerable<SupplierPriceList> allSupplierPriceLists = dataManager.GetPriceLists();
                   Dictionary<int, SupplierPriceList> allSupplierPriceListsDic = new Dictionary<int, SupplierPriceList>();
                   if (allSupplierPriceLists != null)
                   {
                       foreach (var supplierPriceList in allSupplierPriceLists)
                       {
                           allSupplierPriceListsDic.Add(supplierPriceList.PriceListId, supplierPriceList);
                       }
                   }
                   return allSupplierPriceListsDic;
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
