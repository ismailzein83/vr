using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Common;
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
                if (item.IsEnabled && item.Status != Entities.SchedulerTaskStatus.InProgress)
                {
                    if(item.NextRunTime == null)
                    {
                        item.NextRunTime = item.TaskTrigger.CalculateNextTimeToRun();
                    }
                    else
                    {
                        Dictionary<string, object> evaluatedExpressions = item.TaskTrigger.EvaluateExpressions(item);

                        item.Status = Entities.SchedulerTaskStatus.InProgress;
                        dataManager.UpdateTask(item);
                        
                        try
                        {
                            item.TaskAction.Execute(item, evaluatedExpressions);
                            item.Status = Entities.SchedulerTaskStatus.Completed;
                        }
                        catch(Exception ex)
                        {
                            item.Status = Entities.SchedulerTaskStatus.Failed;
                            LoggerFactory.GetExceptionLogger().WriteException(ex);
                        }
                        
                        item.NextRunTime = item.TaskTrigger.CalculateNextTimeToRun();
                        item.LastRunTime = DateTime.Now;
                    }

                    dataManager.UpdateTask(item);
                }
            }
        }
    }
}
