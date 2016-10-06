using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data
{
    public interface ISchedulerTaskDataManager : IDataManager
    {
        IEnumerable<SchedulerTask> GetSchedulerTasks();

        bool AreSchedulerTasksUpdated(ref object updateHandle);
        
        bool AddTask(Entities.SchedulerTask taskObject, out int insertedId);

        bool UpdateTaskInfo(int taskId, string name, bool isEnabled, Guid triggerTypeId, Guid actionTypeId, SchedulerTaskSettings taskSettings);

        bool DeleteTask(int taskId);
    }
}
