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

        public SaleRate CacheAndGetRate(SaleRate rate)
        {
            Dictionary<long, SaleRate> cachedRatesById = this.GetOrCreateObject("cachedRatesById", () => new Dictionary<long, SaleRate>());
            SaleRate matchRate;
            lock (cachedRatesById)
            {
                matchRate = cachedRatesById.GetOrCreateItem(rate.SaleRateId, () => rate);
            }
            return matchRate;
        }

    }
}
