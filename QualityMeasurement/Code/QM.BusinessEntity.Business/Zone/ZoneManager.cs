﻿using QM.BusinessEntity.Data;
using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace QM.BusinessEntity.Business
{
    public class ZoneManager
    {

        public Vanrise.Entities.IDataRetrievalResult<ZoneDetail> GetFilteredZones(Vanrise.Entities.DataRetrievalInput<ZoneQuery> input)
        {
            var allZones = GetAllZones();
            Func<Zone, bool> filterExpression = (prod) =>
                     (input.Query.Name == null || prod.Name.ToLower().Contains(input.Query.Name.ToLower()))
                    && (input.Query.CountryIds == null || input.Query.CountryIds.Contains(prod.CountryId))
                  && ((!input.Query.EffectiveOn.HasValue || (prod.BeginEffectiveDate <= input.Query.EffectiveOn)))
                  && ((!input.Query.EffectiveOn.HasValue || !prod.EndEffectiveDate.HasValue || (prod.EndEffectiveDate > input.Query.EffectiveOn)));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, allZones.ToBigResult(input, filterExpression, ZoneDetailMapper));
        }

        public Dictionary<long, Zone> GetAllZones()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetAllZones", () =>
            {
                IZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneDataManager>();
                IEnumerable<Zone> allZones = dataManager.GetZones();
                Dictionary<long, Zone> allZonesDic = new Dictionary<long, Zone>();
                if (allZones != null)
                {
                    foreach (var zone in allZones)
                    {
                        allZonesDic.Add(zone.ZoneId, zone);
                    }
                }
                return allZonesDic;
            });
        }

        public List<Vanrise.Entities.TemplateConfig> GetZoneSourceTemplates()
        {

            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.SourceZoneReaderConfigType);
        }

        public IEnumerable<ZoneInfo> GetZonesInfo(ZoneInfoFilter filter)
        {
            IEnumerable<Zone> zones = null;

            if (filter != null)
            {
                zones = this.GetZonesByCountry(filter.CountryId);
            }
            else
            {
                var cachedZones = GetCachedZones();
                if (cachedZones != null)
                    zones = cachedZones.Values;
            }

            Func<Zone, bool> filterExpression = (prod) =>
                ((prod.BeginEffectiveDate <= DateTime.Now)
                && (!prod.EndEffectiveDate.HasValue || (prod.EndEffectiveDate > DateTime.Now)) &&(!prod.Settings.IsOffline));

            return zones.MapRecords(ZoneInfoMapper, filterExpression);
        }


        private IEnumerable<Zone> GetZonesByCountry(int? countryId)
        {
            Dictionary<long, Zone> zones = GetCachedZones();

            Func<Zone, bool> filterExpression = (x) =>
                (countryId == null || countryId == x.CountryId);
            return zones.FindAllRecords(filterExpression);
        }



        public void AddZoneFromeSource(Zone zone, List<string> zoneCodes)
        {            
            UpdateSettings(zone, zoneCodes);
            IZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneDataManager>();
            dataManager.InsertZoneFromSource(zone);
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }

        public void UpdateZoneFromeSource(Zone zone, List<string> zoneCodes)
        {
            UpdateSettings(zone, zoneCodes);
            IZoneDataManager dataManager = BEDataManagerFactory.GetDataManager<IZoneDataManager>();
            dataManager.UpdateZoneFromSource(zone);
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }

        private void UpdateSettings(Zone zone, List<string> zoneCodes)
        {
            if(zone.Settings == null)
                zone.Settings = new ZoneSettings();
            if (zone.Settings.ExtendedSettings == null)
                zone.Settings.ExtendedSettings = new Dictionary<string, object>();
            IEnumerable<Type> extendedSettingsBehaviorsImplementations = Utilities.GetAllImplementations<ExtendedZoneSettingBehavior>();
            if (extendedSettingsBehaviorsImplementations != null)
            {
                foreach (var extendedSettingsBehaviorType in extendedSettingsBehaviorsImplementations)
                {
                    var extendedSettingsBehavior = Activator.CreateInstance(extendedSettingsBehaviorType) as ExtendedZoneSettingBehavior;
                    extendedSettingsBehavior.ApplyExtendedSettings(new ApplyExtendedZoneSettingsContext
                    {
                        Zone = zone,
                        ZoneCodes = zoneCodes
                    });
                }
            }
        }

        public Zone GetZone(long zoneId)
        {
            var zones = GetCachedZones();
            return zones.GetRecord(zoneId);
        }

        public Zone GetZonebySourceId(string sourceZoneId)
        {
            Dictionary<long, Zone> zones = GetCachedZones();

            Func<Zone, bool> filterExpression = (x) => (sourceZoneId == x.SourceId);
            return zones.FindRecord(filterExpression);
        }

        #region Private Members

        internal static void ReserveIDRange(int nbOfIds, out long startingId)
        {
            Vanrise.Common.Business.IDManager.Instance.ReserveIDRange(typeof(ZoneManager), nbOfIds, out startingId);
        }

        public Dictionary<string, long> GetExistingItemIds(IEnumerable<string> sourceItemIds)
        {
            Dictionary<string, long> existingItemIds = new Dictionary<string, long>();
            foreach (var item in GetCachedZones())
            {
                if (item.Value.SourceId != null)
                {
                    if (sourceItemIds.Contains(item.Value.SourceId))
                        existingItemIds.Add(item.Value.SourceId, (long)item.Value.ZoneId);
                }
            }
            return existingItemIds;
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

        private ZoneDetail ZoneDetailMapper(Zone zone)
        {
            ZoneDetail zoneDetail = new ZoneDetail();

            zoneDetail.Entity = zone;

            CountryManager manager = new CountryManager();

            int countryId = zone.CountryId;
            var country = manager.GetCountry(countryId);
            zoneDetail.CountryName = (country != null) ? country.Name : "";
            zoneDetail.IsOfflineDescription = zone.Settings.IsOffline ? "Offline" : "Online";

            return zoneDetail;
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
