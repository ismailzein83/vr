using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleZoneCacheManager : Vanrise.Caching.BaseCacheManager
    {
        ISaleZoneDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISaleZoneDataManager>();
        object _updateHandle;

        protected override bool ShouldSetCacheExpired()
        {
            return _dataManager.AreZonesUpdated(ref _updateHandle);
        }
    }
}
