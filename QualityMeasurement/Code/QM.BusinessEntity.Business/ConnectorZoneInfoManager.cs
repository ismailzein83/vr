using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;

namespace QM.BusinessEntity.Business
{
    public class ConnectorZoneInfoManager
    {
        #region Public Methods

        public List<ConnectorZoneInfo> GetZones(string connectorType)
        {
            return GetAllZonesByConnectorType().GetRecord(connectorType);
        }

        public void UpdateZones(string connectorType, IEnumerable<ConnectorZoneInfoToUpdate> zones)
        {
            if (zones == null)
                throw new ArgumentNullException("zones");
            var existingZones = GetZones(connectorType);
            foreach(var zone in zones)
            {
                ConnectorZoneInfo matchZone = existingZones != null ? existingZones.FirstOrDefault(itm => itm.ConnectorZoneId == zone.ConnectorZoneId) : null;
                if (matchZone != null)
                    UpdateZone(connectorType, matchZone.ConnectorZoneInfoId, zone.Codes);
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

        #endregion

        #region Private Methods

        private Dictionary<string, List<ConnectorZoneInfo>> GetAllZonesByConnectorType()
        {
            //cached
            throw new NotImplementedException();
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

        private void UpdateZone(string connectorType, long connectorZoneInfoId, List<string> codes)
        {
            throw new NotImplementedException();
        }

        private void AddZone(string connectorType, string connectorZoneId, List<string> codes)
        {
            throw new NotImplementedException();
        }

        #endregion

        #region Private Classes

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            protected override bool ShouldSetCacheExpired()
            {
                throw new NotImplementedException();
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
