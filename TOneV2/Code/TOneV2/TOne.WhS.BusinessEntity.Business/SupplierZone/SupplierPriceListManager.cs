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
    public class SupplierPriceListManager
    {
      
        #region Public Methods
        public SupplierPriceList GetPriceList(int priceListId)
        {
            List<SupplierPriceList> priceLists = GetCachedPriceLists();
            var priceList = priceLists.FindRecord(x => x.PriceListId == priceListId);
            return priceList;
        }
        public bool SavePriceList(int priceListStatus, DateTime effectiveOnDateTime, string supplierId, string priceListType, string activeSupplierEmail, byte[] contentBytes, string fileName, out int insertdId)
        {
            ISupplierPriceListDataManager dataManager = BEDataManagerFactory.GetDataManager<ISupplierPriceListDataManager>();
            return dataManager.SavePriceList(priceListStatus, effectiveOnDateTime, supplierId, priceListType, activeSupplierEmail, contentBytes, fileName, "Portal", out  insertdId);
        }
        public int GetQueueStatus(int queueId)
        {
            ISupplierPriceListDataManager dataManager =
                BEDataManagerFactory.GetDataManager<ISupplierPriceListDataManager>();
            return dataManager.GetQueueStatus(queueId);
        }
        public IEnumerable<SupplierPriceList> GetFilteredSupplierPriceLists(SupplierPricelistFilter filter)
        {
            List<SupplierPriceList> priceLists = GetCachedPriceLists();
            Func<SupplierPriceList, bool> filterExpression = (item) =>
                (item.SupplierId == filter.SupplierId);

            return priceLists.FindAllRecords(filterExpression);
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
      
        #endregion

    }
}
