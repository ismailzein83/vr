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
        private Dictionary<Guid, RuntimeNodeState> GetRuntimeNodesStates()
        {
            IRuntimeNodeStateDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<IRuntimeNodeStateDataManager>();
            IEnumerable<RuntimeNodeState> data = dataManager.GetAllNodes();
            return data.ToDictionary(cn => cn.RuntimeNodeId, cn => cn);
        }

        public IEnumerable<RuntimeNodeStateDetails> GetAllNodes()
        {
            var allNodes = GetRuntimeNodesStates();
            Func<RuntimeNodeState, bool> filterExpression = (itm) => (true);

            IEnumerable<RuntimeNodeState> nodes = allNodes.FindAllRecords(filterExpression);

            if (nodes == null)
                return null;
            return nodes.MapRecords(RuntimeNodeStateDetailMapper, filterExpression);
        }

        public Vanrise.Runtime.Entities.RuntimeNodeStateDetails GetNodeState(Guid nodeId)
        {
            var allNodes = GetRuntimeNodesStates();
            RuntimeNodeState node;
            allNodes.TryGetValue(nodeId, out node);
            return RuntimeNodeStateDetailMapper(node);
        }



        #region Mapper
        private RuntimeNodeStateDetails RuntimeNodeStateDetailMapper(RuntimeNodeState nodeState)
        {
            return new RuntimeNodeStateDetails
            {
                RuntimeNodeId = nodeState.RuntimeNodeId,
                InstanceId = nodeState.InstanceId,
                MachineName = nodeState.MachineName,
                OSProcessId = nodeState.OSProcessId,
                OSProcessName = nodeState.OSProcessName,
                ServiceURL = nodeState.ServiceURL,
                StartedTimeFormatted = nodeState.StartedTime.ToString(Utilities.GetDateTimeFormat(DateTimeType.DateTime)),
                LastHeartBeatTimeFormatted = nodeState.LastHeartBeatTime.ToString(Utilities.GetDateTimeFormat(DateTimeType.DateTime)),
                NbOfSecondsHeartBeatReceived = nodeState.NbOfSecondsHeartBeatReceived
            };
       
        }
        #endregion
    }
}
