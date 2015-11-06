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
        public SupplierPriceList GetPriceList(int priceListId)
        {
            List<SupplierPriceList> priceLists = GetCachedPriceLists();
            var priceList = priceLists.FindRecord(x => x.PriceListId == priceListId);
            return priceList;
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
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISupplierPriceListDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISupplierPriceListDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.ArGetPriceListsUpdated(ref _updateHandle);
            }
        }
    }
}
