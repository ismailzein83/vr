//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Vanrise.Runtime.Entities;

//namespace Vanrise.Runtime.Data.RDB
//{
//    public class RuntimeNodeStateDataManager : IRuntimeNodeStateDataManager
//    {
//        public List<RuntimeNodeState> GetAllNodes()
//        {
//            throw new NotImplementedException();
//        }

//        public RuntimeNodeState GetNodeState(Guid runtimeNodeId)
//        {
//            throw new NotImplementedException();
//        }

//        public bool TrySetInstanceStarted(Guid runtimeNodeId, Guid serviceInstanceId, string machineName, int osProcessId, string osProcessName, string serviceURL, TimeSpan heartBeatTimeout)
//        {
//            throw new NotImplementedException();
//        }

//        public bool TryUpdateHeartBeat(Guid runtimeNodeId, Guid serviceInstanceId, decimal cpuUsage, decimal availableRAM, string diskInfos, int nbOfEnabledProcesses, int nbOfRunningProcesses)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
