using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Runtime.Data;

namespace Vanrise.Runtime
{
    public class SchedulerService : RuntimeService
    {
        protected override void Execute()
        {
            ISchedulerTaskDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
            List<Entities.SchedulerTask> tasks = dataManager.GetAllTasks();

            foreach (Entities.SchedulerTask item in tasks)
            {
                if (item.IsEnabled && item.Status != Entities.SchedulerTaskStatus.Started && item.TaskTrigger.CheckIfTimeToRun())
                {
                    //TODO: change this to asynchronous
                    dataManager.UpdateTaskStatus(item.TaskId, Entities.SchedulerTaskStatus.Started);
                    item.TaskAction.Execute();
                    dataManager.UpdateTaskStatus(item.TaskId, Entities.SchedulerTaskStatus.Stopped);
                }
                    
            }
        }
    }
}
