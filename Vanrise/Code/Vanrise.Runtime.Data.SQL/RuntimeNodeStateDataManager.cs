using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.SQL
{
    public class RuntimeNodeStateDataManager : BaseSQLDataManager, IRuntimeNodeStateDataManager
    {
        public RuntimeNodeStateDataManager()
            : base(GetConnectionStringName("RuntimeConnStringKey", "RuntimeDBConnString"))
        {
        }

        public bool TrySetInstanceStarted(Guid runtimeNodeId, Guid serviceInstanceId, string machineName, int osProcessId, string osProcessName, string serviceURL, TimeSpan heartBeatTimeout)
        {
            return (bool)ExecuteScalarSP("[runtime].[sp_RuntimeNodeState_TrySetInstanceStarted]", runtimeNodeId, serviceInstanceId, machineName, osProcessId, osProcessName, serviceURL, heartBeatTimeout.TotalSeconds);
        }

        public bool TryUpdateHeartBeat(Guid runtimeNodeId, Guid serviceInstanceId, Decimal cpuUsage, Decimal availableRAM, string diskInfos)
        {
            return ExecuteNonQuerySP("[runtime].[sp_RuntimeNodeState_TryUpdateHeartBeat]", runtimeNodeId, serviceInstanceId, cpuUsage, availableRAM, diskInfos) > 0;
        }

        public RuntimeNodeState GetNodeState(Guid runtimeNodeId)
        {
            return GetItemSP("[runtime].[sp_RuntimeNodeState_GetByNodeID]", RuntimeNodeStateMapper, runtimeNodeId);
        }

        public List<RuntimeNodeState> GetAllNodes()
        {
            return GetItemsSP("[runtime].[sp_RuntimeNodeState_GetAll]", RuntimeNodeStateMapper);
        }

        private RuntimeNodeState RuntimeNodeStateMapper(IDataReader reader)
        {
            return new RuntimeNodeState
            {
                RuntimeNodeId = (Guid)reader["RuntimeNodeID"],
                InstanceId = (Guid)reader["InstanceID"],
                MachineName = reader["MachineName"] as string,
                OSProcessId = GetReaderValue<int>(reader, "OSProcessID"),
                OSProcessName = reader["OSProcessName"] as string,
                ServiceURL = reader["ServiceURL"] as string,
                StartedTime = GetReaderValue<DateTime>(reader, "StartedTime"),
                LastHeartBeatTime = GetReaderValue<DateTime>(reader, "LastHeartBeatTime"),
                NbOfSecondsHeartBeatReceived = Convert.ToDouble(reader["NbOfSecondsHeartBeatReceived"])
            };
        }
    }
}