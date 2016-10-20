using System;
using System.Collections.Generic;
using System.Data;
using Vanrise.Data.SQL;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data.SQL
{
    public class SchedulerTaskDataManager : BaseSQLDataManager, ISchedulerTaskDataManager
    {
        public SchedulerTaskDataManager()
            : base(GetConnectionStringName("RuntimeConfigDBConnStringKey", "RuntimeConfigDBConnString"))
        {

        }
        #region public methods
        public IEnumerable<SchedulerTask> GetSchedulerTasks()
        {
            return GetItemsSP("runtime.sp_SchedulerTask_GetAll", TaskMapper);
        }

        public bool AreSchedulerTasksUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("[runtime].[ScheduleTask]", ref updateHandle);
        }

        public bool AddTask(Entities.SchedulerTask taskObject)
        {

            int recordesEffected = ExecuteNonQuerySP("runtime.sp_SchedulerTask_Insert", taskObject.TaskId, taskObject.Name, 
                taskObject.IsEnabled, taskObject.TaskType, taskObject.TriggerTypeId, taskObject.ActionTypeId,
                Common.Serializer.Serialize(taskObject.TaskSettings), taskObject.OwnerId);
            return (recordesEffected > 0);
        }

        public bool UpdateTaskInfo(Guid taskId, string name, bool isEnabled, Guid triggerTypeId, Guid actionTypeId, SchedulerTaskSettings taskSettings)
        {
            int recordesEffected = ExecuteNonQuerySP("runtime.sp_SchedulerTask_UpdateInfo", taskId, name,
                isEnabled, triggerTypeId, actionTypeId, Common.Serializer.Serialize(taskSettings));
            return (recordesEffected > 0);
        }

        public bool DeleteTask(Guid taskId)
        {
            int recordsEffected = ExecuteNonQuerySP("runtime.sp_SchedulerTask_Delete", taskId);
            return (recordsEffected > 0);
        }
        #endregion

        #region mapper
        SchedulerTask TaskMapper(IDataReader reader)
        {
            SchedulerTask task = new SchedulerTask
            {
                TaskId = GetReaderValue<Guid>(reader,"ID"),
                Name = reader["Name"] as string,
                IsEnabled = bool.Parse(reader["IsEnabled"].ToString()),
                TriggerTypeId = GetReaderValue<Guid>(reader,"TriggerTypeId"),
                ActionTypeId = GetReaderValue<Guid>(reader,"ActionTypeId"),
                TriggerInfo = Common.Serializer.Deserialize<TriggerTypeInfo>(reader["TriggerTypeInfo"] as string),
                ActionInfo = Common.Serializer.Deserialize<ActionTypeInfo>(reader["ActionTypeInfo"] as string),
                TaskSettings = Common.Serializer.Deserialize<SchedulerTaskSettings>(reader["TaskSettings"] as string),
                OwnerId = GetReaderValue<int>(reader, "OwnerId")
            };
            return task;
        }
        #endregion
    }
}
