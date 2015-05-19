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
    public class BusinessEntityInfoManager : IBusinessEntityInfoManager
    {
        public string GetCarrirAccountName(string carrierAccountId)
        {
            TOneCacheManager cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<TOneCacheManager>();
            ConcurrentDictionary<string, string> carrierAccountNames = cacheManager.GetOrCreateObject("CarrierAccountNames",
                TOne.Entities.CacheObjectType.CarrierAccount,
                () => new ConcurrentDictionary<string, string>());
            string carrierAccountName;
            if (!carrierAccountNames.TryGetValue(carrierAccountId, out carrierAccountName))
            {
                ICarrierDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierDataManager>();
                carrierAccountName = dataManager.GetCarrierAccountName(carrierAccountId);
                carrierAccountNames.TryAdd(carrierAccountId, carrierAccountName);
            }
            return carrierAccountName;
        }

        public string GetZoneName(int zoneId)
        {
            TOneCacheManager cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<TOneCacheManager>();
            ConcurrentDictionary<int, string> zoneNames = cacheManager.GetOrCreateObject("ZoneNames",
                TOne.Entities.CacheObjectType.Zone,
                () => new ConcurrentDictionary<int, string>());
            string zoneName;
            if (!zoneNames.TryGetValue(zoneId, out zoneName))
            {
                IZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneDataManager>();
                zoneName = dataManager.GetZoneName(zoneId);
                zoneNames.TryAdd(zoneId, zoneName);
            }
            return zoneName;
        }

        public string GetFlaggedServiceSymbol(short flaggedServiceId)
        {
            TOneCacheManager cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<TOneCacheManager>();
            ConcurrentDictionary<short, string> flaggedServices = cacheManager.GetOrCreateObject("FlaggedServices",
                TOne.Entities.CacheObjectType.FlaggedService, () => new ConcurrentDictionary<short, string>());
            string serviceSymbol;
            if (!flaggedServices.TryGetValue(flaggedServiceId, out serviceSymbol))
            {
                IServiceDataManager dataManager = BEDataManagerFactory.GetDataManager<IServiceDataManager>();
                serviceSymbol = dataManager.GetServiceFlag(flaggedServiceId).Symbol;
            }
            return serviceSymbol;

        }
    }
}
