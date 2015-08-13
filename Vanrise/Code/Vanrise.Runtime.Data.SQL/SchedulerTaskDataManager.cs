using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.SQL
{
    public class SchedulerTaskDataManager : BaseSQLDataManager, ISchedulerTaskDataManager
    {
        public SchedulerTaskDataManager()
            : base(GetConnectionStringName("RuntimeConnStringKey", "RuntimeDBConnString"))
        {

        }

        public Vanrise.Entities.BigResult<Vanrise.Runtime.Entities.SchedulerTask> GetFilteredTasks(Vanrise.Entities.DataRetrievalInput<string> input)
        {
            Action<string> createTempTableAction = (tempTableName) =>
            {
                ExecuteNonQuerySP("runtime.sp_SchedulerTask_CreateTempForFiltered", tempTableName, input.Query);
            };

            return RetrieveData(input, createTempTableAction, TaskMapper);
        }

        public Entities.SchedulerTask GetTask(int taskId)
        {
            return GetItemSP("runtime.sp_SchedulerTask_Get", TaskMapper, taskId);
        }

        public List<SchedulerTask> GetTasksbyActionType(int actionTypeId)
        {
            return GetItemsSP("runtime.sp_SchedulerTask_GetTasksbyActionType", TaskMapper, actionTypeId);
        }

        public List<Entities.SchedulerTask> GetAllTasks()
        {
            return GetItemsSP("runtime.sp_SchedulerTask_GetAll", TaskMapper);
        }

        public List<Entities.SchedulerTask> GetDueTasks()
        {
            return GetItemsSP("runtime.sp_SchedulerTask_GetDueTasks", TaskMapper);
        }

        public bool AddTask(Entities.SchedulerTask taskObject, out int insertedId)
        {
            object taskId;

            int recordesEffected = ExecuteNonQuerySP("runtime.sp_SchedulerTask_Insert", out taskId, taskObject.Name, 
                taskObject.IsEnabled, taskObject.TaskType, SchedulerTaskStatus.NotStarted, taskObject.TriggerTypeId, Common.Serializer.Serialize(taskObject.TaskTrigger),
                taskObject.ActionTypeId, Common.Serializer.Serialize(taskObject.TaskAction));
            insertedId = (int)taskId;
            return (recordesEffected > 0);
        }

        public bool UpdateTask(Entities.SchedulerTask taskObject)
        {
            int recordesEffected = ExecuteNonQuerySP("runtime.sp_SchedulerTask_Update", taskObject.TaskId, taskObject.Name,
                taskObject.IsEnabled, taskObject.Status, taskObject.LastRunTime, taskObject.NextRunTime, taskObject.TriggerTypeId, Common.Serializer.Serialize(taskObject.TaskTrigger),
                taskObject.ActionTypeId, Common.Serializer.Serialize(taskObject.TaskAction));
            return (recordesEffected > 0);
        }

        public bool TryLockTask(int taskId, int currentRuntimeProcessId, IEnumerable<int> runningRuntimeProcessesIds, IEnumerable<Entities.SchedulerTaskStatus> acceptableTaskStatuses)
        {
            int rslt = ExecuteNonQuerySPCmd("runtime.sp_SchedulerTask_TryLockAndUpdateScheduleTask",
                (cmd) =>
                {
                    cmd.Parameters.Add(new SqlParameter("@TaskId", taskId));
                    cmd.Parameters.Add(new SqlParameter("@CurrentRuntimeProcessID", currentRuntimeProcessId));
                    var dtPrm = new SqlParameter("@RunningProcessIDs", SqlDbType.Structured);
                    dtPrm.Value = BuildIDDataTable(runningRuntimeProcessesIds);
                    cmd.Parameters.Add(dtPrm);
                    dtPrm = new SqlParameter("@TaskStatuses", SqlDbType.Structured);
                    dtPrm.Value = BuildIDDataTable(acceptableTaskStatuses.Select(itm => (int)itm));
                    cmd.Parameters.Add(dtPrm);
                });
            return rslt > 0;
        }

        public void UnlockTask(int taskId)
        {
            ExecuteNonQuerySP("runtime.sp_SchedulerTask_UnlockTask", taskId);
        }

        SchedulerTask TaskMapper(IDataReader reader)
        {
            SchedulerTask task = new SchedulerTask
            {
                TaskId = (int)reader["ID"],
                Name = reader["Name"] as string,
                IsEnabled = bool.Parse(reader["IsEnabled"].ToString()),
                Status = (SchedulerTaskStatus) int.Parse(reader["Status"].ToString()),
                LastRunTime =  GetReaderValue<DateTime?>(reader, "LastRunTime"),
                NextRunTime = GetReaderValue<DateTime?>(reader, "NextRunTime"),
                TriggerTypeId = (int)reader["TriggerTypeId"],
                TaskTrigger = Common.Serializer.Deserialize<SchedulerTaskTrigger>(reader["TaskTrigger"] as string),
                ActionTypeId = (int)reader["ActionTypeId"],
                TaskAction = Common.Serializer.Deserialize<SchedulerTaskAction>(reader["TaskAction"] as string)
            };
            return task;
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

    }
}
