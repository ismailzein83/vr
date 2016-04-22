using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Data;

namespace Vanrise.Common.Business
{
    public class BigDataServiceCacheManager : Vanrise.Caching.BaseCacheManager
    {
        IBigDataServiceDataManager _dataManager = CommonDataManagerFactory.GetDataManager<IBigDataServiceDataManager>();
        object _updateHandle;
        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return _dataManager.AreBigDataServicesUpdated(ref _updateHandle);
        }
    }
}
