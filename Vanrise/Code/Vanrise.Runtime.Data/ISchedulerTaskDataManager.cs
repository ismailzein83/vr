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
        Vanrise.Entities.BigResult<Vanrise.Runtime.Entities.SchedulerTask> GetFilteredTasks(Vanrise.Entities.DataRetrievalInput<string> input);

        Entities.SchedulerTask GetTask(int taskId);

        List<SchedulerTask> GetTasksbyActionType(int actionTypeId);

        List<Entities.SchedulerTask> GetAllTasks();

        List<Entities.SchedulerTask> GetDueTasks();

        bool AddTask(Entities.SchedulerTask taskObject, out int insertedId);

        bool UpdateTask(Entities.SchedulerTask taskObject);

        bool UpdateTaskInfo(int taskId, string name, bool isEnabled, int triggerTypeId, int actionTypeId, SchedulerTaskSettings taskSettings);

        bool DeleteTask(int taskId);

        bool TryLockTask(int taskId, int currentRuntimeProcessId, IEnumerable<int> runningRuntimeProcessesIds);

        void UnlockTask(int taskId);
    }
}
