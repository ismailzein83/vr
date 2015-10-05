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
        public List<SaleZonePackage> GetSaleZonePackages()
        {
            ISaleZonePackageDataManager dataManager = BEDataManagerFactory.GetDataManager<ISaleZonePackageDataManager>();
            return dataManager.GetSaleZonePackages();
        }

        public List<SaleZonePackage> GetCachedSaleZonePackages()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SaleZonePackageCacheManager>().GetOrCreateObject("GetCachedSaleZonePackages",
                () => GetSaleZonePackages());
        }
    }
}
