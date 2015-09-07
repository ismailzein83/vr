using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Runtime.Data;
using Vanrise.Runtime.Entities;

namespace Vanrise.Runtime
{
    public class SchedulerService : RuntimeService
    {
        protected override void Execute()
        {
            ISchedulerTaskDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
            List<Entities.SchedulerTask> tasks = dataManager.GetDueTasks();

            RunningProcessManager runningProcessManager = new RunningProcessManager();
            IEnumerable<int> runningRuntimeProcessesIds = runningProcessManager.GetCachedRunningProcesses(new TimeSpan(0, 0, 15)).Select(itm => itm.ProcessId);

            int currentRuntimeProcessId = RunningProcessManager.CurrentProcess.ProcessId;

            foreach (Entities.SchedulerTask item in tasks)
            {
                if (item.IsEnabled && dataManager.TryLockTask(item.TaskId, currentRuntimeProcessId, runningRuntimeProcessesIds))
                {
                    Task task = new Task(() =>
                        {
                            SchedulerTaskTrigger taskTrigger = null;
                            try
                            {
                                taskTrigger = (SchedulerTaskTrigger)Activator.CreateInstance(Type.GetType(item.TriggerInfo.FQTN));
                                if (item.NextRunTime != null)
                                {
                                    Dictionary<string, object> evaluatedExpressions = taskTrigger.EvaluateExpressions(item);

                                    item.Status = Entities.SchedulerTaskStatus.InProgress;
                                    dataManager.UpdateTask(item);

                                    SchedulerTaskAction taskAction = (SchedulerTaskAction)Activator.CreateInstance(Type.GetType(item.ActionInfo.FQTN));

                                    taskAction.Execute(item, item.TaskSettings.TaskActionArgument, evaluatedExpressions);
                                    item.Status = Entities.SchedulerTaskStatus.Completed;
                                }
                            }
                            catch(Exception ex)
                            {
                                item.Status = Entities.SchedulerTaskStatus.Failed;
                                LoggerFactory.GetExceptionLogger().WriteException(ex);
                            }
                            finally
                            {
                                if(taskTrigger != null)
                                {
                                    item.NextRunTime = taskTrigger.CalculateNextTimeToRun(item.TaskSettings.TaskTriggerArgument);
                                    item.LastRunTime = DateTime.Now;
                                }
                                
                                dataManager.UpdateTask(item);
                                dataManager.UnlockTask(item.TaskId);
                            }
                        }
                    );

                    task.Start();
                }
            }
        }
    }
}
