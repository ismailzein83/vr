using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityServiceCacheManager : Vanrise.Caching.BaseCacheManager
    {
        private ISaleEntityServiceDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityServiceDataManager>();
        private object _updateHandle;

        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return _dataManager.AreSaleEntityServicesUpdated(ref _updateHandle);
        }
    }
}
