using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Vanrise.Common;
using Vanrise.Runtime.Business;
using Vanrise.Runtime.Entities;
using Vanrise.Security.Entities;

namespace Vanrise.Runtime
{
    public class SchedulerService : RuntimeService
    {
        SchedulerTaskStateManager _scheduleTaskStateManager = new SchedulerTaskStateManager();
        SchedulerTaskManager _scheduleTaskManager = new SchedulerTaskManager();
        IUserManager _userManager = Vanrise.Security.Entities.BEManagerFactory.GetManager<IUserManager>();

        public override Guid ConfigId { get { return new Guid("2BB63679-43F1-4859-A883-5CA48009A8D1"); } }

        static int s_maxConcurrentTasks;

        static SchedulerService()
        {
            if (!int.TryParse(ConfigurationManager.AppSettings["Runtime_SchedulerService_MaxConcurrentTasks"], out s_maxConcurrentTasks))
                s_maxConcurrentTasks = 3;
        }

        public override void Execute()
        {
            List<Entities.SchedulerTaskState> schedulerTaskStates = GetDueTasks();
            if (schedulerTaskStates == null)
                return;

            ConcurrentQueue<Entities.SchedulerTaskState> qTaskStates = new ConcurrentQueue<SchedulerTaskState>(schedulerTaskStates);

            Parallel.For(0, Math.Min(s_maxConcurrentTasks, qTaskStates.Count),
                (i) =>
                {
                    Entities.SchedulerTaskState schedulerTaskState;
                    while(qTaskStates.TryDequeue(out schedulerTaskState))
                    {
                        SchedulerTask schedulerTask = _scheduleTaskManager.GetTask(schedulerTaskState.TaskId);
                        if (schedulerTask == null)
                        {
                            Vanrise.Common.LoggerFactory.GetLogger().WriteWarning("No Scheduled Task found for the Id: '{0}'", schedulerTaskState.TaskId);
                            _scheduleTaskStateManager.DeleteTaskState(schedulerTaskState.TaskId);
                            continue;
                        }
                        int? systemUserId = _userManager.GetSystemUserId();
                        int userId = systemUserId.HasValue ? systemUserId.Value : schedulerTask.OwnerId;
                        Vanrise.Security.Entities.ContextFactory.GetContext().SetContextUserId(userId);
                        if (!IsTaskEnabledAndEffective(schedulerTask))
                        {
                            if (schedulerTaskState.Status == SchedulerTaskStatus.WaitingEvent && CheckIfScheduleTaskCompleted(schedulerTaskState, schedulerTask))
                            {
                                schedulerTaskState.NextRunTime = null;
                                _scheduleTaskStateManager.UpdateTaskState(schedulerTaskState);
                            }
                        }
                        else
                        {
                            TryLockAndExecuteTask(schedulerTask, schedulerTaskState, userId);
                        }
                    }
                });
        }

        private bool IsTaskEnabledAndEffective(SchedulerTask schedulerTask)
        {
            return schedulerTask.IsEnabled && schedulerTask.TaskSettings.StartEffDate <= DateTime.Now && (!schedulerTask.TaskSettings.EndEffDate.HasValue || schedulerTask.TaskSettings.EndEffDate.Value > DateTime.Now);
        }

        private void TryLockAndExecuteTask(SchedulerTask schedulerTask, Entities.SchedulerTaskState schedulerTaskState, int userId)
        {
            TransactionLocker.Instance.TryLock(String.Concat("SchedulerTask_", schedulerTaskState.TaskId),
                () =>
                {                    
                    Vanrise.Security.Entities.ContextFactory.GetContext().SetContextUserId(userId);
                    SchedulerTaskTrigger taskTrigger = (SchedulerTaskTrigger)Activator.CreateInstance(Type.GetType(schedulerTask.TriggerInfo.FQTN));
                    bool updateTaskState = false;
                    try
                    {
                        if (schedulerTaskState.Status == SchedulerTaskStatus.WaitingEvent)
                        {
                            updateTaskState = CheckIfScheduleTaskCompleted(schedulerTaskState, schedulerTask);
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

                                Dictionary<string, object> evaluatedExpressions = taskTrigger.EvaluateExpressions(schedulerTask, schedulerTaskState);

                                schedulerTaskState.Status = Entities.SchedulerTaskStatus.InProgress;
                                _scheduleTaskStateManager.UpdateTaskState(schedulerTaskState);

                                SchedulerTaskExecuteOutput taskExecuteOutput = new SchedulerTaskExecuteOutput();
                                schedulerTaskState.LastRunTime = DateTime.Now;
                                taskExecuteOutput = taskAction.Execute(schedulerTask, schedulerTask.TaskSettings.TaskActionArgument, evaluatedExpressions);
                                if (taskExecuteOutput != null)
                                    schedulerTaskState.ExecutionInfo = taskExecuteOutput.ExecutionInfo;
                                if (taskExecuteOutput == null || taskExecuteOutput.Result == ExecuteOutputResult.Completed)
                                    schedulerTaskState.Status = SchedulerTaskStatus.Completed;
                                else
                                    schedulerTaskState.Status = SchedulerTaskStatus.WaitingEvent;
                                updateTaskState = true;
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

                            _scheduleTaskStateManager.UpdateTaskState(schedulerTaskState);
                        }
                    }
                });
        }

        private bool CheckIfScheduleTaskCompleted(Entities.SchedulerTaskState schedulerTaskState, SchedulerTask schedulerTask)
        {
            bool updateTaskState = false;
            SchedulerTaskAction taskAction = (SchedulerTaskAction)Activator.CreateInstance(Type.GetType(schedulerTask.ActionInfo.FQTN));
            var checkProgressContext = new SchedulerTaskCheckProgressContext
            {
                Task = schedulerTask,
                ExecutionInfo = schedulerTaskState.ExecutionInfo
            };
            SchedulerTaskCheckProgressOutput output = taskAction.CheckProgress(checkProgressContext, schedulerTask.OwnerId);

            if (output.Result == ExecuteOutputResult.Completed)
            {
                schedulerTaskState.Status = SchedulerTaskStatus.Completed;
                updateTaskState = true;
            }
            return updateTaskState;
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