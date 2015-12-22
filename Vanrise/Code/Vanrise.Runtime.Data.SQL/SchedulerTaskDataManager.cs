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
                ExecuteNonQuerySP("runtime.sp_SchedulerTask_CreateTempByName", tempTableName, input.Query);
            };

            return RetrieveData(input, createTempTableAction, TaskMapper);
        }

        public Entities.SchedulerTask GetTask(int taskId)
        {
            return GetItemSP("runtime.sp_SchedulerTask_Get", TaskMapper, taskId);
        }

        public List<SchedulerTask> GetTasksbyActionType(int actionTypeId)
        {
            return GetItemsSP("runtime.sp_SchedulerTask_GetByActionTypeId", TaskMapper, actionTypeId);
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
                taskObject.IsEnabled, taskObject.TaskType, SchedulerTaskStatus.NotStarted, taskObject.TriggerTypeId, taskObject.ActionTypeId,
                Common.Serializer.Serialize(taskObject.TaskSettings), taskObject.OwnerId);
            insertedId = (int)taskId;
            return (recordesEffected > 0);
        }

        public bool UpdateTask(Entities.SchedulerTask taskObject)
        {
            int recordesEffected = ExecuteNonQuerySP("runtime.sp_SchedulerTask_Update", taskObject.TaskId, taskObject.Name,
                taskObject.IsEnabled, taskObject.Status, taskObject.LastRunTime, taskObject.NextRunTime, taskObject.TriggerTypeId, taskObject.ActionTypeId,
                Common.Serializer.Serialize(taskObject.TaskSettings), taskObject.ExecutionInfo);
            return (recordesEffected > 0);
        }

        public bool UpdateTaskInfo(int taskId, string name, bool isEnabled, int triggerTypeId, int actionTypeId, SchedulerTaskSettings taskSettings)
        {
            int recordesEffected = ExecuteNonQuerySP("runtime.sp_SchedulerTask_UpdateInfo", taskId, name,
                isEnabled, triggerTypeId, actionTypeId, Common.Serializer.Serialize(taskSettings));
            return (recordesEffected > 0);
        }

        public bool DeleteTask(int taskId)
        {
            int recordsEffected = ExecuteNonQuerySP("runtime.sp_SchedulerTask_Delete", taskId);
            return (recordsEffected > 0);
        }

        public bool TryLockTask(int taskId, int currentRuntimeProcessId, IEnumerable<int> runningRuntimeProcessesIds)
        {
            int rslt = ExecuteNonQuerySPCmd("runtime.sp_SchedulerTask_TryLockAndUpdateScheduleTask",
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
                ActionTypeId = (int)reader["ActionTypeId"],
                TriggerInfo = Common.Serializer.Deserialize<TriggerTypeInfo>(reader["TriggerTypeInfo"] as string),
                ActionInfo = Common.Serializer.Deserialize<ActionTypeInfo>(reader["ActionTypeInfo"] as string),
                TaskSettings = Common.Serializer.Deserialize<SchedulerTaskSettings>(reader["TaskSettings"] as string),
                OwnerId = GetReaderValue<int>(reader, "OwnerId"),
                ExecutionInfo = GetReaderValue<string>(reader, "ExecutionInfo")
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
