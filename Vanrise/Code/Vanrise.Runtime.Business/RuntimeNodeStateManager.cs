using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;
using Vanrise.Common;
using Vanrise.Entities;

namespace Vanrise.Runtime.Business
{
    public class RuntimeNodeStateManager
    {
        private Dictionary<Guid, RuntimeNodeState> GetCachedRuntimeNodesStates()
        {
            IRuntimeNodeStateDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeNodeStateDataManager>();
            IEnumerable<RuntimeNodeState> data = dataManager.GetAllNodes();
            return data.ToDictionary(cn => cn.RuntimeNodeId, cn => cn);
        }

        public List<RuntimeNodeState> GetAllNodes()
        {
            var allNodes = GetCachedRuntimeNodesStates();
            Func<RuntimeNodeState, bool> filterExpression = (itm) => (true);

            IEnumerable<RuntimeNodeState> nodes = allNodes.FindAllRecords(filterExpression);
            if (nodes == null)
                return null;
            return nodes.ToList();
        }

        public Vanrise.Runtime.Entities.RuntimeNodeState GetNodeState(Guid nodeId)
        {
            var allNodes = GetCachedRuntimeNodesStates();
            RuntimeNodeState node;
            allNodes.TryGetValue(nodeId, out node);
            return node;
        }

        //public Dictionary<Guid, RuntimeNodeState> GetAllNodes()
        //{
        //    return Vanrise.Caching.CacheManagerFactory.GetCacheManager<CacheManager>()
        //  .GetOrCreateObject("GetCacheRuntimeNodesState", () =>
        //  {
        //      var allNodes = s_dataManager.GetAllNodes();
        //      return allNodes != null ? allNodes.ToDictionary(n => n.RuntimeNodeId, n => n) : null;
        //  });
        //}


        //public RuntimeNodeState GetNodeState(Guid nodeId)
        //{
        //    return GetAllNodes().GetRecord(nodeId);
        //}


        #region Mapper
        private RuntimeNodeStateDetails RuntimeNodeStateDetailMapper(RuntimeNodeStateDetails nodeState)
        {

            return new RuntimeNodeStateDetails
            {
                RuntimeNodeId = nodeState.RuntimeNodeId,
                InstanceId = nodeState.InstanceId,
                MachineName = nodeState.MachineName,
                OSProcessId = nodeState.OSProcessId,
                OSProcessName = nodeState.OSProcessName,
                ServiceURL = nodeState.ServiceURL,
                StartedTime = nodeState.StartedTime,
                LastHeartBeatTime = nodeState.LastHeartBeatTime,
                NbOfSecondsHeartBeatReceived = nodeState.NbOfSecondsHeartBeatReceived
            };
       
        }
        #endregion
    }
}
