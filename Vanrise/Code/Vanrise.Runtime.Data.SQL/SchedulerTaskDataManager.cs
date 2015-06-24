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

        public bool AddTask(Entities.SchedulerTask taskObject, out int insertedId)
        {
            object taskId;

            int recordesEffected = ExecuteNonQuerySP("runtime.sp_Scheduler_Insert", out taskId, taskObject.Name, taskObject.IsEnabled);
            insertedId = (int)taskId;
            return (recordesEffected > 0);
        }

        public bool UpdateTask(Entities.SchedulerTask taskObject)
        {
            int recordesEffected = ExecuteNonQuerySP("runtime.sp_Scheduler_Update", taskObject.TaskId, taskObject.Name, taskObject.IsEnabled);
            return (recordesEffected > 0);
        }
        

        SchedulerTask TaskMapper(IDataReader reader)
        {
            SchedulerTask task = new SchedulerTask
            {
                TaskId = (int)reader["ID"],
                Name = reader["Name"] as string,
                IsEnabled = bool.Parse(reader["IsEnabled"].ToString())
            };
            return task;
        }

    }
}
