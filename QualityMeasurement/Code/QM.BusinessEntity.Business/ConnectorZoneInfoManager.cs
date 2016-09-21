using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.BusinessEntity.Data;
using Vanrise.Common;
using Vanrise.Common.Business;

namespace QM.BusinessEntity.Business
{
    public class ConnectorZoneInfoManager
    {
        #region Public Methods

        public void UpdateZones(string connectorType, IEnumerable<ConnectorZoneInfoToUpdate> zones)
        {
            if (zones == null)
                throw new ArgumentNullException("zones");
            var existingZones = GetZones(connectorType);
            foreach(var zone in zones)
            {
                ConnectorZoneInfo matchZone = existingZones != null ? existingZones.FirstOrDefault(itm => itm.ConnectorZoneId == zone.ConnectorZoneId) : null;
                if (matchZone != null)
                    UpdateZone(matchZone.ConnectorZoneInfoId, zone.Codes);
                else
                    AddZone(connectorType, zone.ConnectorZoneId, zone.Codes);
            }
            Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().SetCacheExpired();
        }

        public ConnectorZoneInfo GetLongestMatchZone(string connectorType, IEnumerable<string> codes)
        {
            if (connectorType == null)
                throw new ArgumentNullException("connectorType");
            if (codes == null)
                throw new ArgumentNullException("codes");
            var zonesReverseOrderedByCode = GetZonesReverseOrdered(connectorType);
            if (zonesReverseOrderedByCode == null)
                return null;
            foreach(var code in codes.OrderByDescending(c => c.Length))
            {
                var zone = zonesReverseOrderedByCode.FirstOrDefault(itm => itm.Code.StartsWith(code) || code.StartsWith(itm.Code));
                if (zone != null)
                    return zone.Zone;
            }
            return null;
        }

        public IEnumerable<ConnectorZoneReaderConfig> GetConnectorZoneTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<ConnectorZoneReaderConfig>(ConnectorZoneReaderConfig.EXTENSION_TYPE);
        }

        #endregion

        #region Private Methods

        private List<ConnectorZoneInfo> GetZones(string connectorType)
        {
            return GetAllZonesByConnectorType().GetRecord(connectorType);
        }

        private Dictionary<string, List<ConnectorZoneInfo>> GetAllZonesByConnectorType()
        {
            Dictionary<string, List<ConnectorZoneInfo>> connectorZonesInfo = new Dictionary<string, List<ConnectorZoneInfo>>();
            foreach (var item in GetCachedConnectorZonesInfo())
            {
                List<ConnectorZoneInfo> listconnectorZoneInfo = new List<ConnectorZoneInfo>();
                if (!connectorZonesInfo.ContainsKey((item.Value.ConnectorType)))
                {
                    
                    listconnectorZoneInfo.Add(item.Value);
                    connectorZonesInfo.Add(item.Value.ConnectorType, listconnectorZoneInfo);
                }
                else
                {
                    connectorZonesInfo.TryGetValue(item.Value.ConnectorType, out listconnectorZoneInfo);
                    listconnectorZoneInfo.Add(item.Value);
                    connectorZonesInfo[item.Value.ConnectorType] = listconnectorZoneInfo;
                }
            }
                
            return connectorZonesInfo;
        }

        private List<ConnectorZoneInfoSingleCode> GetZonesReverseOrdered(string connectorType)
        {
            string cacheName = string.Format("GetZonesReverseOrdered_{0}", connectorType);
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject(cacheName,
                () =>
                {
                    var zones = GetZones(connectorType);
                    if (zones != null)
                    {
                        return zones.SelectMany(zone => zone.Codes != null ? zone.Codes.Select(code => new ConnectorZoneInfoSingleCode { Code = code, Zone = zone }) : null).OrderByDescending(itm => itm.Code).ToList();
                    }
                    else
                        return null;
                });
        }

        private bool UpdateZone(long connectorZoneInfoId, List<string> codes)
        {
            IConnectorZoneInfoDataManager dataManager = BEDataManagerFactory.GetDataManager<IConnectorZoneInfoDataManager>();
            return dataManager.UpdateZone(connectorZoneInfoId,codes);
        }

        private bool AddZone(string connectorType, string connectorZoneId, List<string> codes)
        {
            IConnectorZoneInfoDataManager dataManager = BEDataManagerFactory.GetDataManager<IConnectorZoneInfoDataManager>();
            return dataManager.AddZone(connectorType, connectorZoneId, codes);
        }

        private Dictionary<long, ConnectorZoneInfo> GetCachedConnectorZonesInfo()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetConnectorZonesInfo",
               () =>
               {
                   IConnectorZoneInfoDataManager dataManager = BEDataManagerFactory.GetDataManager<IConnectorZoneInfoDataManager>();
                   IEnumerable<ConnectorZoneInfo> connectorZonesInfo = dataManager.GetConnectorZonesInfo();
                   return connectorZonesInfo.ToDictionary(cn => cn.ConnectorZoneInfoId, cn => cn);
               });
        }
        
        #endregion

        #region Private Classes
        
        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IConnectorZoneInfoDataManager _dataManager = BEDataManagerFactory.GetDataManager<IConnectorZoneInfoDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreConnectorZonesInfoUpdated(ref _updateHandle);
            }
        }

        private class ConnectorZoneInfoSingleCode
        {
            public ConnectorZoneInfo Zone { get; set; }

            public string Code { get; set; }
        }

        #endregion
    }
}
