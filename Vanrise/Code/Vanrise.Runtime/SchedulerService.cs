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
            List<Entities.SchedulerTask> tasks = dataManager.GetReadyAndNewTasks();

            foreach (Entities.SchedulerTask item in tasks)
            {
                if (item.IsEnabled && item.Status != Entities.SchedulerTaskStatus.Started)
                {
                    if(item.NextRunTime == null)
                    {
                        item.NextRunTime = item.TaskTrigger.CalculateNextTimeToRun();
                    }
                    else
                    {
                        Dictionary<string, string> evaluatedExpressions = item.TaskTrigger.EvaluateExpressions(item);

                        item.Status = Entities.SchedulerTaskStatus.Started;
                        dataManager.UpdateTask(item);

                        item.TaskAction.Execute(evaluatedExpressions);

                        item.Status = Entities.SchedulerTaskStatus.NotStarted;
                        item.NextRunTime = item.TaskTrigger.CalculateNextTimeToRun();
                        item.LastRunTime = DateTime.Now;
                    }

                    dataManager.UpdateTask(item);
                }
            }
        }
    }
}
