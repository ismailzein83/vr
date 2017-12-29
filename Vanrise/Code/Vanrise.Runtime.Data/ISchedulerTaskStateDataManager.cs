using System;
using System.Collections.Generic;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime.Data
{
    public interface ISchedulerTaskStateDataManager : IDataManager
    {
        List<SchedulerTaskState> GetSchedulerTaskStateByTaskIds(List<Guid> taskIds);

        SchedulerTaskState GetSchedulerTaskStateByTaskId(Guid taskId);
        
        void UnlockTask(Guid taskId);

        bool TryLockTask(Guid taskId, int currentRuntimeProcessId, IEnumerable<int> runningRuntimeProcessesIds);

        bool UpdateTaskState(SchedulerTaskState taskStateObject);

        List<SchedulerTaskState> GetAllScheduleTaskStates();

        void InsertSchedulerTaskState(Guid taskId);

        void DeleteTaskState(Guid taskId);

        void RunSchedulerTask(Guid taskId);
    }
}
