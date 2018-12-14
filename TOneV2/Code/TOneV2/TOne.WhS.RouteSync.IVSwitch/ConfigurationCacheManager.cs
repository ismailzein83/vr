using System;
using NP.IVSwitch.Entities;
using TOne.WhS.BusinessEntity.Business;

namespace TOne.WhS.RouteSync.IVSwitch
{
    public class ConfigurationCacheManager : Vanrise.Caching.BaseCacheManager
    {
        DateTime? _carrierProfileLastCheck;
        DateTime? _switchLastCheck;
        DateTime? _routeLastCheck;
        DateTime? _enpPointLastCheck;
        protected override bool IsTimeExpirable
        {
            get { return true; }
        }
        protected override bool ShouldSetCacheExpired(object parameter)
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CarrierAccountManager.CacheManager>().IsCacheExpired(ref _carrierProfileLastCheck)
                   | Vanrise.Caching.CacheManagerFactory.GetCacheManager<SwitchManager.CacheManager>().IsCacheExpired(ref _switchLastCheck)
                   | NPManagerFactory.GetManager<IRouteManager>().IsCacheExpired(ref _routeLastCheck)
                   | NPManagerFactory.GetManager<IEndPointManager>().IsCacheExpired(ref _enpPointLastCheck);

        }
    }
}
