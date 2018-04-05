using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data
{
    public interface IRuntimeNodeStateDataManager : IDataManager
    {
        bool TrySetInstanceStarted(Guid runtimeNodeId, Guid serviceInstanceId, string machineName, int osProcessId, string osProcessName, string serviceURL, TimeSpan heartBeatTimeout);

        bool TryUpdateHeartBeat(Guid runtimeNodeId, Guid serviceInstanceId, Decimal cpuUsage, Decimal availableRAM, string diskInfos);

        RuntimeNodeState GetNodeState(Guid runtimeNodeId);

        List<RuntimeNodeState> GetAllNodes();
    }
}
