using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Entities;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.SQL
{
    public class RunningProcessDataManager : BaseSQLDataManager, IRunningProcessDataManager
    {
        public RunningProcessDataManager()
            : base(GetConnectionStringName("RuntimeConnStringKey", "RuntimeDBConnString"))
        {
        }

        public Entities.RunningProcessInfo InsertProcessInfo(Guid runtimeNodeId, Guid runtimeNodeInstanceId, int osProcessId, RunningProcessAdditionalInfo additionalInfo)
        {
            return GetItemSP("runtime.sp_RunningProcess_Insert", RunningProcessInfoMapper, runtimeNodeId, runtimeNodeInstanceId, osProcessId, additionalInfo != null ? Common.Serializer.Serialize(additionalInfo) : null);
        }

        public List<RunningProcessInfo> GetRunningProcesses()
        {
            return GetItemsSP("[runtime].[sp_RunningProcess_GetAll]", RunningProcessInfoMapper);
        }

        public List<Entities.RunningProcessDetails> GetFilteredRunningProcesses(DataRetrievalInput<RunningProcessQuery> input)
        {
            return GetItemsSP("[runtime].[sp_RunningProcess_GetFiltered]", RunningProcessDetailsMapper, input.Query.RuntimeNodeInstanceId);
        }

        public void DeleteRunningProcess(int runningProcessId)
        {
            ExecuteNonQuerySP("[runtime].[sp_RunningProcess_Delete]", runningProcessId);
        }

        public void GetRunningProcessSummary(out int? maxProcessId, out int processCount)
        {
            int? maxProcessId_local = null;
            int processCount_local = 0;
            ExecuteReaderSP("runtime.sp_RunningProcess_GetMaxIdAndCount",
                (reader) =>
                {
                    if(reader.Read())
                    {
                        maxProcessId_local = GetReaderValue<int?>(reader, "MaxID");
                        processCount_local = GetReaderValue<int>(reader, "NbOfProcesses");
                    }
                });
            maxProcessId = maxProcessId_local;
            processCount = processCount_local;
        }

        public void SetRunningProcessReady(int processId)
        {
            ExecuteNonQuerySP("runtime.sp_RunningProcess_SetReady", processId);
        }

        public RunningProcessInfo GetRunningProcess(int processId)
        {
            return GetItemSP("[runtime].[sp_RunningProcess_GetByID]", RunningProcessInfoMapper, processId);
        }

        #region Private Methods

        private RunningProcessInfo RunningProcessInfoMapper(IDataReader reader)
        {
            var runningProcessInfo = new RunningProcessInfo
            {
                ProcessId = (int)reader["ID"],
                OSProcessId = GetReaderValue<int>(reader, "OSProcessID"),
                RuntimeNodeId = GetReaderValue<Guid>(reader, "RuntimeNodeID"),
                RuntimeNodeInstanceId = GetReaderValue<Guid>(reader, "RuntimeNodeInstanceID"),
                StartedTime = (DateTime)reader["StartedTime"]
            };
            string serializedAdditionInfo = reader["AdditionalInfo"] as string;
            if (serializedAdditionInfo != null)
                runningProcessInfo.AdditionalInfo = Common.Serializer.Deserialize(serializedAdditionInfo) as RunningProcessAdditionalInfo;
            return runningProcessInfo;
        }

        private RunningProcessDetails RunningProcessDetailsMapper(IDataReader reader)
        {
            var runningProcessDetails = new RunningProcessDetails
            {
                ProcessId = (int)reader["ID"],
                OSProcessId = GetReaderValue<int>(reader, "OSProcessID"),
                RuntimeNodeId = GetReaderValue<Guid>(reader, "RuntimeNodeID"),
                RuntimeNodeInstanceId = GetReaderValue<Guid>(reader, "RuntimeNodeInstanceID"),
                StartedTime = (DateTime)reader["StartedTime"]
            };
            string serializedAdditionInfo = reader["AdditionalInfo"] as string;
            if (serializedAdditionInfo != null)
                runningProcessDetails.AdditionalInfo = Common.Serializer.Deserialize(serializedAdditionInfo) as RunningProcessAdditionalInfo;
            return runningProcessDetails;
        }

        #endregion
    }
}
