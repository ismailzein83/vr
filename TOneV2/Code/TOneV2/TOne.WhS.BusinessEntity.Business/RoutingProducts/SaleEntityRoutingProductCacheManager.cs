using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleEntityRoutingProductCacheManager : Vanrise.Caching.BaseCacheManager
    {
        ISaleEntityRoutingProductDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISaleEntityRoutingProductDataManager>();
        object _updateHandle;

        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return _dataManager.AreSaleEntityRoutingProductUpdated(ref _updateHandle);
        }
    }
}
