using QM.BusinessEntity.Data;
using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common.Business;
using Vanrise.Common;

namespace QM.BusinessEntity.Business
{
    public class ZoneManager
    {
        public List<Vanrise.Entities.TemplateConfig> GetZoneSourceTemplates()
        {

            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.SourceZoneReaderConfigType);
        }

        public IEnumerable<ZoneInfo> GetZonesInfo()
        {
            var zones = GetCachedZones();
            return zones.MapRecords(ZoneInfoMapper);
        }

       
        #region Private Members

        internal static void ReserveIDRange(int nbOfIds, out long startingId)
        {
            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(typeof(ZoneManager), nbOfIds, out startingId);
        }


        public Dictionary<long, Zone> GetCachedZones()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetZones",
               () =>
               {
                   IZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneDataManager>();
                   IEnumerable<Zone> zones = dataManager.GetZones();
                   return zones.ToDictionary(cn => cn.ZoneId, cn => cn);
               });
        }

        private ZoneInfo ZoneInfoMapper(Zone zone)
        {
            return new ZoneInfo()
            {
                ZoneId = zone.ZoneId,
                Name = zone.Name,
            };
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

    }
}
