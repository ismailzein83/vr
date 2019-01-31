using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SMSBusinessEntity.Data;
using Vanrise.Caching;

namespace TOne.WhS.SMSBusinessEntity.Business
{
    public class CustomerSMSRateCacheManager : BaseCacheManager
    {
        ICustomerSMSRateDataManager _dataManager = CustomerSMSRateDataManagerFactory.GetDataManager<ICustomerSMSRateDataManager>();
        object _updateHandle;

        public override Vanrise.Caching.CacheObjectSize ApproximateObjectSize
        {
            get
            {
                return Vanrise.Caching.CacheObjectSize.ExtraLarge;
            }
        }

        public override T GetOrCreateObject<T>(object cacheName, Func<T> createObject)
        {
            return GetOrCreateObject(cacheName, BECacheExpirationChecker.Instance, createObject);
        }

        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return _dataManager.AreSaleRatesUpdated(ref _updateHandle);
        }
    }
}
