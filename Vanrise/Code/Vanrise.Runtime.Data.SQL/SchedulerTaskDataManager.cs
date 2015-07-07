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
    public class SchedulerTaskDataManager : BaseSQLDataManager, ISchedulerTaskDataManager
    {
        public SchedulerTaskDataManager()
            : base(GetConnectionStringName("RuntimeConnStringKey", "RuntimeDBConnString"))
        {

        }

        public List<Entities.SchedulerTask> GetFilteredTasks(int fromRow, int toRow, string name)
        {
            return GetItemsSP("runtime.sp_SchedulerTask_GetFiltered", TaskMapper, fromRow, toRow, name);
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

        public List<Entities.SchedulerTask> GetReadyAndNewTasks()
        {
            return GetItemsSP("runtime.sp_SchedulerTask_GetReadyAndNewTasks", TaskMapper);
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

    }
}
