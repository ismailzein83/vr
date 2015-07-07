using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.BusinessEntity.Data;
using TOne.BusinessEntity.Entities;
using TOne.Caching;

namespace TOne.BusinessEntity.Business
{
    public class ZoneManager
    { 
        IZoneDataManager _dataManager;

        public ZoneManager()
        {
            _dataManager = BEDataManagerFactory.GetDataManager<IZoneDataManager>();
        }
        public void LoadZonesInfo(DateTime effectiveTime, bool isFuture, List<Entities.CarrierAccountInfo> activeSuppliers, int batchSize, Action<List<Entities.ZoneInfo>> onBatchAvailable)
        {
            _dataManager.LoadZonesInfo(effectiveTime, isFuture, activeSuppliers, batchSize, onBatchAvailable);
        }
        public List<ZoneInfo> GetSalesZones(string nameFilter)
        {
            return GetZones("SYS", nameFilter);
        }
        public List<ZoneInfo> GetZoneList(IEnumerable<int> zonesIds)
        {
            return _dataManager.GetZoneList(zonesIds);
        }
        public List<ZoneInfo> GetZones(string supplierId, string nameFilter)
        {
            return _dataManager.GetZones(supplierId, nameFilter);
        }

        public Dictionary<int, Zone> GetAllZones()
        {
            TOneCacheManager cacheManager = Vanrise.Caching.CacheManagerFactory.GetCacheManager<TOneCacheManager>();
            return cacheManager.GetOrCreateObject("AllZones",
                TOne.Entities.CacheObjectType.Zone,
                () =>
                {
                    return _dataManager.GetAllZones();
                });
        }

        public Zone GetZone(int zoneId)
        {
            Zone zone;
            var allZones = GetAllZones();
            if (allZones != null)
                allZones.TryGetValue(zoneId, out zone);
            else
                zone = null;
            return zone;
        }
    }
}
