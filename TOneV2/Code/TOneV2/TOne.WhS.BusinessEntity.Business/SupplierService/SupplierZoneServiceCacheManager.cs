using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneServiceCacheManager : Vanrise.Caching.BaseCacheManager
    {
        private ISupplierZoneServiceDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISupplierZoneServiceDataManager>();
        private object _updateHandle;

        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return _dataManager.AreSupplierZoneServicesUpdated(ref _updateHandle);
        }
    }
}
