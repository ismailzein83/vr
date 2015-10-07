using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleZonePackageManager
    {
        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            ISaleZonePackageDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISaleZonePackageDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreZonePackagesUpdated(ref _updateHandle);
            }
        }

        #endregion

        public List<SaleZonePackage> GetSaleZonePackages()
        {
            return GetCachedSaleZonePackages();

        }

        #region Private Method

        List<SaleZonePackage> GetCachedSaleZonePackages()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetSaleZonePackages",
               () =>
               {
                   ISaleZonePackageDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleZonePackageDataManager>();
                   return dataManager.GetSaleZonePackages();
               });

        }

        #endregion
    }
}
