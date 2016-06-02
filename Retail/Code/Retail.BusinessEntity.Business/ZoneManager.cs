using Retail.BusinessEntity.Data;
using Retail.BusinessEntity.Entities;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;

namespace Retail.BusinessEntity.Business
{
    public class ZoneManager
    {
        #region ctor/Local Variables
        #endregion

        #region Public Methods
        public List<Zone> GetZonesByCountryId(int countryId)
        {
            var items = GetCachedZonesByCountryId();
            return items.GetRecord(countryId);
        }
        #endregion

        #region Private Members
        private Dictionary<int, Zone> GetCachedZones()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetZones",
               () =>
               {
                   IZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneDataManager>();
                   IEnumerable<Zone> zones = dataManager.GetZones();
                   return zones.ToDictionary(cn => cn.ZoneId, cn => cn);
               });
        }
        private Dictionary<int, List<Zone>> GetCachedZonesByCountryId()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetZonesByCountryId",
               () =>
               {
                   Dictionary<int, List<Zone>> data = new Dictionary<int, List<Zone>>();
                   IZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneDataManager>();
                   IEnumerable<Zone> zones = dataManager.GetZones();
                   if (zones != null)
                       foreach (Zone zone in zones)
                       {
                           List<Zone> relatedZones;
                           if (data.TryGetValue(zone.CountryId, out relatedZones))
                           {
                               relatedZones.Add(zone);
                           }
                           else
                           {
                               relatedZones = new List<Zone>() { zone };
                               data.Add(zone.CountryId, relatedZones);
                           }
                       }
                   return data;
               });
        }
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IZoneDataManager _dataManager = BEDataManagerFactory.GetDataManager<IZoneDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return _dataManager.AreZonesUpdated(ref _updateHandle);
            }

        }

        #endregion

        #region  Mappers

        #endregion
    }
}
