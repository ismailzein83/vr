using System;
using System.Collections.Generic;
using System.Configuration;
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
        static int s_maxConcurrentExecutingHeavyTasks;
        static int s_nbOfExecutingHeavyTasks;
        static Object s_lockObj = new object();
        static TimeSpan s_TryExecuteHeavyTaskWaitTurnInterval = TimeSpan.FromMilliseconds(300);
        static int s_TryExecuteHeavyTaskMaxRetryCount = 4;

        static SchedulerService()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["Runtime_SchedulerService_MaxConcurrentExecutingHeavyTasks"], out s_maxConcurrentExecutingHeavyTasks))
                s_maxConcurrentExecutingHeavyTasks = 1;
        }

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
                            bool isExecutionPostponed = false;
                            bool isHeavyTask = false;
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
                                        taskAction.CheckProgress(checkProgressContext, item.OwnerId);
                                    if (output.Result == ExecuteOutputResult.Completed)
                                        item.Status = SchedulerTaskStatus.Completed;
                                }
                                else
                                {
                                    taskTrigger = (SchedulerTaskTrigger)Activator.CreateInstance(Type.GetType(item.TriggerInfo.FQTN));
                                    if (item.NextRunTime != null)
                                    {
                                        SchedulerTaskAction taskAction = (SchedulerTaskAction)Activator.CreateInstance(Type.GetType(item.ActionInfo.FQTN));
                                        isHeavyTask = taskAction.IsHeavyTask;

                                        if(isHeavyTask)
                                        {
                                            int retry = 0;
                                            while(retry < s_TryExecuteHeavyTaskMaxRetryCount)
                                            {
                                                lock (s_lockObj)
                                                {
                                                    if (s_nbOfExecutingHeavyTasks >= s_maxConcurrentExecutingHeavyTasks)
                                                        isExecutionPostponed = true;
                                                    else
                                                    {
                                                        isExecutionPostponed = false;
                                                        s_nbOfExecutingHeavyTasks++;
                                                        break;
                                                    }
                                                }
                                                retry++;
                                                Thread.Sleep(s_TryExecuteHeavyTaskWaitTurnInterval);
                                            }
                                            
                                        }

                                        if (!isExecutionPostponed)
                                        {
                                            Dictionary<string, object> evaluatedExpressions = taskTrigger.EvaluateExpressions(item);

                                            item.Status = Entities.SchedulerTaskStatus.InProgress;
                                            dataManager.UpdateTask(item);

                                            SchedulerTaskExecuteOutput taskExecuteOutput = new SchedulerTaskExecuteOutput();
                                            Console.WriteLine("Executing Task Id: {0}, Type: {1}", item.TaskId, taskAction.GetType().Name);
                                            taskExecuteOutput = taskAction.Execute(item, item.TaskSettings.TaskActionArgument, evaluatedExpressions);
                                            Console.WriteLine("Task Id: {0}, Type: {1} Executed", item.TaskId, taskAction.GetType().Name);
                                            if (taskExecuteOutput != null)
                                                item.ExecutionInfo = taskExecuteOutput.ExecutionInfo;
                                            if (taskExecuteOutput == null || taskExecuteOutput.Result == ExecuteOutputResult.Completed)
                                                item.Status = SchedulerTaskStatus.Completed;
                                            else
                                                item.Status = SchedulerTaskStatus.WaitingEvent;
                                            item.LastRunTime = DateTime.Now;
                                        }
                                        else
                                            Console.WriteLine("Task Id: {0}, Type: {1} Postponed", item.TaskId, taskAction.GetType().Name);
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
                                if (!isExecutionPostponed)
                                {
                                    if (taskTrigger != null)
                                    {
                                        item.NextRunTime = taskTrigger.CalculateNextTimeToRun(item, item.TaskSettings.TaskTriggerArgument);
                                    }
                                    dataManager.UpdateTask(item);
                                }
                                
                                if(isHeavyTask && !isExecutionPostponed)
                                {
                                    lock(s_lockObj)
                                    {
                                        s_nbOfExecutingHeavyTasks--;
                                    }
                                }                                
                                
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
