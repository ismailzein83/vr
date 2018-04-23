using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;
using Vanrise.Common;

namespace Vanrise.Runtime.Business
{
    public class RuntimeNodeManager
    {
        static IRuntimeNodeDataManager s_dataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeNodeDataManager>();

        private class CacheManager : Vanrise.Caching.BaseCacheManager
        {
            IRuntimeNodeDataManager runtimeNodeDataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeNodeDataManager>();
            object _updateHandle;
            protected override bool ShouldSetCacheExpired(object parameter)
            {
                return runtimeNodeDataManager.AreRuntimeNodeUpdated(ref _updateHandle);
            }
        }

        public Dictionary<Guid, RuntimeNode> GetAllNodes()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
          .GetOrCreateObject("GetCacheRuntimeNodesConfigurations", () =>
          {
              var allNodes = s_dataManager.GetAllNodes();
              return allNodes != null ? allNodes.ToDictionary(n => n.RuntimeNodeId, n => n) : null;
          });
        }

        public RuntimeNode GetNode(Guid nodeId)
        {
            return GetAllNodes().GetRecord(nodeId);
        }
    }
}
