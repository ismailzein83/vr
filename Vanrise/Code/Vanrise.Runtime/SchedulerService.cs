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
                if (item.IsEnabled && item.TaskSettings.StartEffDate < DateTime.Now && (item.TaskSettings.EndEffDate == null || item.TaskSettings.EndEffDate > DateTime.Now) &&
                    dataManager.TryLockTask(item.TaskId, currentRuntimeProcessId, runningRuntimeProcessesIds))
                {
                    Task task = new Task(() =>
                        {
                            SchedulerTaskTrigger taskTrigger = null;
                            try
                            {
                                if ( item.Status == SchedulerTaskStatus.WaitingEvent)
                                {
                                    SchedulerTaskAction taskAction = (SchedulerTaskAction)Activator.CreateInstance(Type.GetType(item.ActionInfo.FQTN));
                                    var checkProgressContext = new SchedulerTaskCheckProgressContext
                                    {
                                        ExecutionInfo = item.ExecutionInfo
                                    };
                                    SchedulerTaskCheckProgressOutput output =
                                        taskAction.CheckProgress(checkProgressContext);
                                    if (output.Result == ExecuteOutputResult.Completed)
                                        item.Status = SchedulerTaskStatus.Completed;
                                }
                                else
                                {
                                    taskTrigger = (SchedulerTaskTrigger)Activator.CreateInstance(Type.GetType(item.TriggerInfo.FQTN));
                                    if (item.NextRunTime != null)
                                    {
                                        Dictionary<string, object> evaluatedExpressions = taskTrigger.EvaluateExpressions(item);

                                        item.Status = Entities.SchedulerTaskStatus.InProgress;
                                        dataManager.UpdateTask(item);

                                        SchedulerTaskAction taskAction = (SchedulerTaskAction)Activator.CreateInstance(Type.GetType(item.ActionInfo.FQTN));

                                        SchedulerTaskExecuteOutput taskExecuteOutput = new SchedulerTaskExecuteOutput();
                                        taskExecuteOutput = taskAction.Execute(item, item.TaskSettings.TaskActionArgument, evaluatedExpressions);
                                        if (taskExecuteOutput != null)
                                            item.ExecutionInfo = taskExecuteOutput.ExecutionInfo;
                                        if(taskExecuteOutput == null || taskExecuteOutput.Result == ExecuteOutputResult.Completed)
                                            item.Status = SchedulerTaskStatus.Completed;
                                        else
                                            item.Status = SchedulerTaskStatus.WaitingEvent;
                                        item.LastRunTime = DateTime.Now;
                                    }
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
                                    item.NextRunTime = taskTrigger.CalculateNextTimeToRun(item, item.TaskSettings.TaskTriggerArgument);
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
