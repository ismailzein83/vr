using System.Collections.Generic;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data
{
    public interface ISchedulerTaskStateDataManager : IDataManager
    {
        List<SchedulerTaskState> GetSchedulerTaskStateByTaskIds(List<int> taskIds);

        SchedulerTaskState GetSchedulerTaskStateByTaskId(int taskId);
        List<SchedulerTaskState> GetDueTasks();
        void UnlockTask(int taskId);

        bool TryLockTask(int taskId, int currentRuntimeProcessId, IEnumerable<int> runningRuntimeProcessesIds);

        bool UpdateTaskState(SchedulerTaskState taskStateObject);

        List<SchedulerTaskState> GetAllScheduleTaskStates();

        void InsertSchedulerTaskState(int taskId);
    }
}
