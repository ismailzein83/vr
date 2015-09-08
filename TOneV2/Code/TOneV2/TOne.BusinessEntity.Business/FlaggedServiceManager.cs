using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using TOne.Caching;

namespace TOne.BusinessEntity.Business
{
    public class FlaggedServiceManager
    {
        IFlaggedServiceDataManager _dataManager;
        public FlaggedServiceManager()
        {
            _dataManager = BEDataManagerFactory.GetDataManager<IFlaggedServiceDataManager>();
        }

        #region Private Methods
        private List<FlaggedService> GetCachedMatchFlaggedServices(short serviceFlagId)
        {
            TOneCacheManager cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<TOneCacheManager>();
            ConcurrentDictionary<short, List<FlaggedService>> cachedMatchFlaggedService = cacheManager.GetOrCreateObject("MatchFlaggedService",
                TOne.Entities.CacheObjectType.FlaggedService,
                () => new ConcurrentDictionary<short, List<FlaggedService>>());
            List<FlaggedService> matchServices;
            if (!cachedMatchFlaggedService.TryGetValue(serviceFlagId, out matchServices))
            {
                matchServices = GetMatchedFlaggedServices(serviceFlagId);
                cachedMatchFlaggedService.TryAdd(serviceFlagId, matchServices);
            }
            return matchServices;
        }

        private List<FlaggedService> GetMatchedFlaggedServices(short serviceFlagId)
        {
            var flaggedServices = GetServiceFlags();
            List<FlaggedService> rslt = new List<FlaggedService>();
            if (serviceFlagId == 0)
            {
                rslt.Add(flaggedServices[0]);
                return rslt;
            }

            foreach (FlaggedService service in flaggedServices)
                if ((service.FlaggedServiceID & serviceFlagId) == service.FlaggedServiceID)
                {
                    rslt.Add(service);
                }
            return rslt;
        }

        #endregion

        #region Public Methods

        public List<FlaggedService> GetServiceFlags()
        {
            TOneCacheManager cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<TOneCacheManager>();
            return cacheManager.GetOrCreateObject("GetServiceFlags",
                TOne.Entities.CacheObjectType.FlaggedService,
                () =>
                {
                    return _dataManager.GetServiceFlags();
                }).Values.ToList();
        }

        public void AssignFlaggedServiceInfo(IFlaggedServiceEntity entity)
        {
            var matchFlaggedServices = GetCachedMatchFlaggedServices(entity.FlaggedServiceID);
            if (matchFlaggedServices != null && matchFlaggedServices.Count > 0)
            {
                entity.FlaggedServiceColor = String.Join(",", matchFlaggedServices.Select(itm => itm.ServiceColor));
                entity.FlaggedServiceSymbol = String.Join(",", matchFlaggedServices.Select(itm => itm.Symbol));
            }
        }
        public void AssignFlaggedServiceInfo(IEnumerable<IFlaggedServiceEntity> entities)
        {
            if (entities != null)
            {
                foreach (var entity in entities)
                {
                    AssignFlaggedServiceInfo(entity);
                }
            }
        }
        #endregion
    }
}
