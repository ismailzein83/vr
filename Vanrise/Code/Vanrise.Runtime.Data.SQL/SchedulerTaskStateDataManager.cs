using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Vanrise.Data.SQL;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.SQL
{
    public class SchedulerTaskStateDataManager : BaseSQLDataManager, ISchedulerTaskStateDataManager
    {
        #region Ctor/Properties

        public SchedulerTaskStateDataManager()
            : base(GetConnectionStringName("RuntimeConnStringKey", "RuntimeDBConnString"))
        {

        }

        #endregion

        #region public methods

        public List<SchedulerTaskState> GetSchedulerTaskStateByTaskIds(List<Guid> taskIds)
        {
            string taskIdsAsString = null;
            if (taskIds != null && taskIds.Count() > 0)
                taskIdsAsString = string.Join<Guid>(",", taskIds);

            return GetItemsSP("runtime.sp_SchedulerTaskState_GetByTaskIds", TaskStateMapper, taskIdsAsString);
        }

        public SchedulerTaskState GetSchedulerTaskStateByTaskId(Guid taskId)
        {
            return GetItemSP("runtime.sp_SchedulerTaskState_GetByTaskId", TaskStateMapper, taskId);
        }

        public void UnlockTask(Guid taskId)
        {
            ExecuteNonQuerySP("runtime.sp_SchedulerTaskState_UnlockTask", taskId);
        }

        public bool TryLockTask(Guid taskId, int currentRuntimeProcessId, IEnumerable<int> runningRuntimeProcessesIds)
        {
            int rslt = ExecuteNonQuerySPCmd("runtime.sp_SchedulerTaskState_TryLockAndUpdateScheduleTask",
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@TaskId", taskId));
                    cmd.Parameters.Add(new SqlParameter("@CurrentRuntimeProcessID", currentRuntimeProcessId));
                    var dtPrm = new SqlParameter("@RunningProcessIDs", SqlDbType.Structured);
                    dtPrm.Value = BuildIDDataTable(runningRuntimeProcessesIds);
                    cmd.Parameters.Add(dtPrm);
                });
            return rslt > 0;
        }

        public bool UpdateTaskState(SchedulerTaskState taskStateObject)
        {
            int recordesEffected = ExecuteNonQuerySP("runtime.sp_SchedulerTaskState_Update", taskStateObject.TaskId, (int)taskStateObject.Status, taskStateObject.NextRunTime,
                taskStateObject.LastRunTime, taskStateObject.ExecutionInfo != null ? Common.Serializer.Serialize(taskStateObject.ExecutionInfo) : default(string));
            return (recordesEffected > 0);
        }

        public List<SchedulerTaskState> GetAllScheduleTaskStates()
        {
            return GetItemsSP("runtime.sp_SchedulerTaskState_GetAll", TaskStateMapper);
        }

        public void InsertSchedulerTaskState(Guid taskId)
        {
            ExecuteNonQuerySP("runtime.sp_SchedulerTaskState_Insert", taskId);
        }

        public void DeleteTaskState(Guid taskId)
        {
            ExecuteNonQuerySP("runtime.sp_SchedulerTaskState_Delete", taskId);
        }

        public void RunSchedulerTask(Guid taskId, bool allowRunIfEnabled)
        {
            ExecuteNonQuerySP("runtime.sp_SchedulerTaskState_RunSchedulerTask", taskId, DateTime.Now, allowRunIfEnabled ? 1 : 0);
        }

        #endregion

        #region mapper
        SchedulerTaskState TaskStateMapper(IDataReader reader)
        {
            return new SchedulerTaskState
            {
                TaskId = GetReaderValue<Guid>(reader, "TaskId"),
                Status = (SchedulerTaskStatus)reader["Status"],
                LastRunTime = GetReaderValue<DateTime?>(reader, "LastRunTime"),
                NextRunTime = GetReaderValue<DateTime?>(reader, "NextRunTime"),
                ExecutionInfo = reader["ExecutionInfo"] == DBNull.Value ? null : Common.Serializer.Deserialize<Object>(reader["ExecutionInfo"] as string)
            };
        }
        DataTable BuildIDDataTable<T>(IEnumerable<T> ids)
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("ID", typeof(T));
            dt.BeginLoadData();
            if (ids != null)
            {
                foreach (var id in ids)
                {
                    dt.Rows.Add(id);
                }
            }
            dt.EndLoadData();
            return dt;
        }
        #endregion
    }
}