using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Runtime.Business;
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
            List<Entities.SchedulerTaskState> schedulerTaskStates = GetDueTasks();
            if (schedulerTaskStates == null)
                return;

            SchedulerTaskStateManager scheduleTaskStateManager = new SchedulerTaskStateManager();
            SchedulerTaskManager scheduleTaskManager = new SchedulerTaskManager();

            RunningProcessManager runningProcessManager = new RunningProcessManager();
            IEnumerable<int> runningRuntimeProcessesIds = runningProcessManager.GetCachedRunningProcesses().Select(itm => itm.ProcessId);

            int currentRuntimeProcessId = RunningProcessManager.CurrentProcess.ProcessId;

            foreach (Entities.SchedulerTaskState schedulerTaskState in schedulerTaskStates)
            {
                SchedulerTask schedulerTask = scheduleTaskManager.GetTask(schedulerTaskState.TaskId);
                if (schedulerTask == null)
                {
                    Vanrise.Common.LoggerFactory.GetLogger().WriteWarning("No Scheduled Task found for the Id: '{0}'", schedulerTaskState.TaskId);
                    scheduleTaskStateManager.DeleteTaskState(schedulerTaskState.TaskId);
                    continue;
                }

                if (schedulerTask.IsEnabled && schedulerTask.TaskSettings.StartEffDate < DateTime.Now && (schedulerTask.TaskSettings.EndEffDate == null || schedulerTask.TaskSettings.EndEffDate > DateTime.Now) &&
                    scheduleTaskStateManager.TryLockTask(schedulerTaskState.TaskId, currentRuntimeProcessId, runningRuntimeProcessesIds))
                {
                    Task task = new Task(() =>
                    {
                        Vanrise.Security.Entities.ContextFactory.GetContext().SetContextUserId(schedulerTask.OwnerId);
                        bool isExecutionPostponed = false;
                        bool isHeavyTask = false;
                        SchedulerTaskTrigger taskTrigger = (SchedulerTaskTrigger)Activator.CreateInstance(Type.GetType(schedulerTask.TriggerInfo.FQTN));
                        bool updateTaskState = false;
                        try
                        {
                            if (schedulerTaskState.Status == SchedulerTaskStatus.WaitingEvent)
                            {
                                SchedulerTaskAction taskAction = (SchedulerTaskAction)Activator.CreateInstance(Type.GetType(schedulerTask.ActionInfo.FQTN));
                                var checkProgressContext = new SchedulerTaskCheckProgressContext
                                {
                                    Task = schedulerTask,
                                    ExecutionInfo = schedulerTaskState.ExecutionInfo
                                };
                                SchedulerTaskCheckProgressOutput output =
                                    taskAction.CheckProgress(checkProgressContext, schedulerTask.OwnerId);
                                if (output.Result == ExecuteOutputResult.Completed)
                                {
                                    schedulerTaskState.Status = SchedulerTaskStatus.Completed;
                                    updateTaskState = true;
                                }
                            }
                            else
                            {
                                if (!schedulerTaskState.NextRunTime.HasValue)
                                {
                                    updateTaskState = true;
                                }
                                else
                                {
                                    SchedulerTaskAction taskAction = (SchedulerTaskAction)Activator.CreateInstance(Type.GetType(schedulerTask.ActionInfo.FQTN));
                                    isHeavyTask = taskAction.IsHeavyTask;

                                    if (isHeavyTask)
                                    {
                                        int retry = 0;
                                        while (retry < s_TryExecuteHeavyTaskMaxRetryCount)
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
                                        Dictionary<string, object> evaluatedExpressions = taskTrigger.EvaluateExpressions(schedulerTask, schedulerTaskState);

                                        schedulerTaskState.Status = Entities.SchedulerTaskStatus.InProgress;
                                        scheduleTaskStateManager.UpdateTaskState(schedulerTaskState);

                                        SchedulerTaskExecuteOutput taskExecuteOutput = new SchedulerTaskExecuteOutput();
                                        Console.WriteLine("Executing Task Id: {0}, Type: {1}", schedulerTaskState.TaskId, taskAction.GetType().Name);
                                        schedulerTaskState.LastRunTime = DateTime.Now;
                                        taskExecuteOutput = taskAction.Execute(schedulerTask, schedulerTask.TaskSettings.TaskActionArgument, evaluatedExpressions);
                                        Console.WriteLine("Task Id: {0}, Type: {1} Executed", schedulerTaskState.TaskId, taskAction.GetType().Name);
                                        if (taskExecuteOutput != null)
                                            schedulerTaskState.ExecutionInfo = taskExecuteOutput.ExecutionInfo;
                                        if (taskExecuteOutput == null || taskExecuteOutput.Result == ExecuteOutputResult.Completed)
                                            schedulerTaskState.Status = SchedulerTaskStatus.Completed;
                                        else
                                            schedulerTaskState.Status = SchedulerTaskStatus.WaitingEvent;
                                        updateTaskState = true;
                                    }
                                    else
                                        Console.WriteLine("Task Id: {0}, Type: {1} Postponed", schedulerTaskState.TaskId, taskAction.GetType().Name);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            schedulerTaskState.Status = Entities.SchedulerTaskStatus.Failed;
                            updateTaskState = true;
                            LoggerFactory.GetExceptionLogger().WriteException(ex);
                        }
                        finally
                        {
                            if (updateTaskState)
                            {
                                if (schedulerTaskState.Status != SchedulerTaskStatus.WaitingEvent)
                                    schedulerTaskState.NextRunTime = taskTrigger.CalculateNextTimeToRun(schedulerTask, schedulerTaskState, schedulerTask.TaskSettings.TaskTriggerArgument);
                                scheduleTaskStateManager.UpdateTaskState(schedulerTaskState);
                            }

                            if (isHeavyTask && !isExecutionPostponed)
                            {
                                lock (s_lockObj)
                                {
                                    s_nbOfExecutingHeavyTasks--;
                                }
                            }

                            scheduleTaskStateManager.UnlockTask(schedulerTaskState.TaskId);
                        }
                    }
                    );

                    task.Start();
                }
            }
        }

        private List<SchedulerTaskState> GetDueTasks()
        {
            List<SchedulerTaskState> dueTasks = new List<SchedulerTaskState>();
            SchedulerTaskStateManager scheduleTaskStateManager = new SchedulerTaskStateManager();
            SchedulerTaskManager scheduleTaskManager = new SchedulerTaskManager();

            List<SchedulerTask> allTasks = scheduleTaskManager.GetAllScheduleTasks();
            if (allTasks == null || allTasks.Count == 0)
                return null;

            List<SchedulerTaskState> allTaskStates = scheduleTaskStateManager.GetAllScheduleTaskStates();

            IEnumerable<SchedulerTask> newTasks = allTasks.FindAll(itm => allTaskStates == null || allTaskStates.FirstOrDefault(item => item.TaskId == itm.TaskId) == null);

            DateTime now = DateTime.Now;

            if (allTaskStates != null)
            {
                IEnumerable<SchedulerTaskState> existingDueTasks = allTaskStates.FindAll(itm => !itm.NextRunTime.HasValue || itm.NextRunTime.Value <= now);
                if (existingDueTasks != null)
                    dueTasks.AddRange(existingDueTasks);
            }

            if (newTasks != null)
            {
                List<SchedulerTask> newTaskList = newTasks.ToList();
                foreach (SchedulerTask task in newTaskList)
                {
                    dueTasks.Add(new SchedulerTaskState(task.TaskId, SchedulerTaskStatus.NotStarted, null, null, null));
                    scheduleTaskStateManager.InsertSchedulerTaskState(task.TaskId);
                }
            }

            return dueTasks.Count > 0 ? dueTasks : null;
        }
    }
}