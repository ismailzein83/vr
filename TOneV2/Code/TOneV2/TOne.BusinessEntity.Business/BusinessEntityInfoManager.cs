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
                ICarrierAccountDataManager dataManager = BEDataManagerFactory.GetDataManager<ICarrierAccountDataManager>();
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
        
        public string GetSwitchName(int switchId)
        {
            TOneCacheManager cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<TOneCacheManager>();
            ConcurrentDictionary<int, string> switcNames = cacheManager.GetOrCreateObject("SwitchNames",
                TOne.Entities.CacheObjectType.Switch,
                () => new ConcurrentDictionary<int, string>());
            string switchName;
            if (!switcNames.TryGetValue(switchId, out switchName))
            {
                ISwitchDataManager dataManager = BEDataManagerFactory.GetDataManager<ISwitchDataManager>();
                switchName = dataManager.GetSwitchName(switchId);
                switcNames.TryAdd(switchId, switchName);
            }
            return switchName;
        }

        public string GetCodeGroupName(int codeGroupId)
        {
            TOneCacheManager cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<TOneCacheManager>();
            ConcurrentDictionary<int, string> codeGroupNames = cacheManager.GetOrCreateObject("CodeGroupNames",
                TOne.Entities.CacheObjectType.CodeGroup,
                () => new ConcurrentDictionary<int, string>());
            string codeGroupName;
            if (!codeGroupNames.TryGetValue(codeGroupId, out codeGroupName))
            {
                ICodeDataManager dataManager = BEDataManagerFactory.GetDataManager<ICodeDataManager>();
                codeGroupName = dataManager.GetCodeGroupName(codeGroupId);
                codeGroupNames.TryAdd(codeGroupId, codeGroupName);
            }
            return codeGroupName;
        }
    }
}
