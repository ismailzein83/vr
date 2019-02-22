﻿using System;
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

        bool AreSchedulerTasksUpdated(ref object lastReceivedDataInfo);

        bool AddTask(Entities.SchedulerTask taskObject);

        bool UpdateTaskInfo(Guid taskId, string name, bool isEnabled, Guid triggerTypeId, Guid actionTypeId, SchedulerTaskSettings taskSettings);

        bool DeleteTask(Guid taskId);

        bool DisableTask(Guid taskId);

        bool EnableTask(Guid taskId);
    }
}
