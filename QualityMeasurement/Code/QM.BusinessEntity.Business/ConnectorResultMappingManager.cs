using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using QM.BusinessEntity.Data;
using QM.BusinessEntity.Entities;
using Vanrise.Common;

namespace QM.BusinessEntity.Business
{
    public class ConnectorResultMappingManager
    {
        #region Private Classes

        public List<ConnectorResultMapping> GetConnectorResultMappings(string connectorType)
        {
            return GetAllZonesByConnectorType().GetRecord(connectorType);
        }
        private Dictionary<string, List<ConnectorResultMapping>> GetAllZonesByConnectorType()
        {
            Dictionary<string, List<ConnectorResultMapping>> connectorResultMapping = new Dictionary<string, List<ConnectorResultMapping>>();
            foreach (var item in GetCachedconnectorResultsMapping())
            {
                List<ConnectorResultMapping> listConnectorResultMapping = new List<ConnectorResultMapping>();
                if (!connectorResultMapping.ContainsKey((item.Value.ConnectorType)))
                {

                    listConnectorResultMapping.Add(item.Value);
                    connectorResultMapping.Add(item.Value.ConnectorType, listConnectorResultMapping);
                }
                else
                {
                    connectorResultMapping.TryGetValue(item.Value.ConnectorType, out listConnectorResultMapping);
                    listConnectorResultMapping.Add(item.Value);
                    connectorResultMapping[item.Value.ConnectorType] = listConnectorResultMapping;
                }
            }

            return connectorResultMapping;
        }

        private Dictionary<int, ConnectorResultMapping> GetCachedconnectorResultsMapping()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>().GetOrCreateObject("GetconnectorResultsMapping",
               () =>
               {
                   IConnectorResultMappingDataManager dataManager = BEDataManagerFactory.GetDataManager<IConnectorResultMappingDataManager>();
                   IEnumerable<ConnectorResultMapping> connectorResultMapping = dataManager.GetConnectorResultMappings();
                   return connectorResultMapping.ToDictionary(cn => cn.ConnectorResultMappingId, cn => cn);
               });
        }

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IConnectorResultMappingDataManager _dataManager = BEDataManagerFactory.GetDataManager<IConnectorResultMappingDataManager>();
            object _updateHandle;

            protected override bool ShouldSetCacheExpired()
            {
                return _dataManager.AreConnectorResultMappingUpdated(ref _updateHandle);
            }
        }
        #endregion
    }
}
