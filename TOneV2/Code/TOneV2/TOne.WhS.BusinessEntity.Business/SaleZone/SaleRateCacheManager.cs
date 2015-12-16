using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Data;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleRateCacheManager : Vanrise.Caching.BaseCacheManager
    {
        ISaleRateDataManager _dataManager = BEDataManagerFactory.GetDataManager<ISaleRateDataManager>();
        object _updateHandle;

        public override Vanrise.Caching.CacheObjectSize ApproximateObjectSize
        {
            get
            {
                return Vanrise.Caching.CacheObjectSize.ExtraLarge;
            }
        }

        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return _dataManager.AreSaleRatesUpdated(ref _updateHandle);
        }
    }
}
