using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.SQL
{
    public class RunningProcessDataManager : BaseSQLDataManager, IRunningProcessDataManager
    {
        public RunningProcessDataManager()
            : base(ConfigurationManager.AppSettings["RuntimeConnStringKey"] ?? "RuntimeDBConnString")
        {
        }

        public Entities.RunningProcessInfo InsertProcessInfo(string processName, string machineName)
        {
            return GetItemSP("runtime.sp_RunningProcess_Insert", RunningProcessInfoMapper, processName, machineName);
        }

        public bool UpdateHeartBeat(int processId, out DateTime heartBeatTime)
        {
            object obj;
            if (ExecuteNonQuerySP("runtime.sp_RunningProcess_UpdateHeartBeat", out obj, processId) > 0)
            {
                heartBeatTime = (DateTime)obj;
                return true;
            }
            else
            {
                heartBeatTime = default(DateTime);
                return false;
            }
        }

        public void DeleteTimedOutProcesses(TimeSpan heartBeatReceivedBefore)
        {
            ExecuteNonQuerySP("runtime.sp_RunningProcess_DeleteOld", heartBeatReceivedBefore.TotalSeconds);
        }

        public List<Entities.RunningProcessInfo> GetRunningProcesses(TimeSpan? heartBeatReceivedWithin)
        {
            return GetItemsSP("runtime.sp_RunningProcess_GetByHeartBeat", RunningProcessInfoMapper, heartBeatReceivedWithin != null ? (object)heartBeatReceivedWithin.Value.TotalSeconds : DBNull.Value);
        }

        #region Private Methods

        private RunningProcessInfo RunningProcessInfoMapper(IDataReader reader)
        {
            return new RunningProcessInfo
            {
                ProcessId = (int)reader["ID"],
                ProcessName = reader["ProcessName"] as string,
                MachineName = reader["MachineName"] as string,
                StartedTime = (DateTime)reader["StartedTime"],
                LastHeartBeatTime = (DateTime)reader["LastHeartBeatTime"],
            };
        }

        #endregion
    }
}
