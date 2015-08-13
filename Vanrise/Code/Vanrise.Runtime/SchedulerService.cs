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
        private static IEnumerable<SchedulerTaskStatus> _acceptableTaskStatuses = new SchedulerTaskStatus[] { SchedulerTaskStatus.NotStarted, SchedulerTaskStatus.Failed, SchedulerTaskStatus.Completed };

        protected override void Execute()
        {
            ISchedulerTaskDataManager dataManager = RuntimeDataManagerFactory.GetDataManager<ISchedulerTaskDataManager>();
            List<Entities.SchedulerTask> tasks = dataManager.GetDueTasks();

            RunningProcessManager runningProcessManager = new RunningProcessManager();
            IEnumerable<int> runningRuntimeProcessesIds = runningProcessManager.GetCachedRunningProcesses(new TimeSpan(0, 0, 15)).Select(itm => itm.ProcessId);

            int currentRuntimeProcessId = RunningProcessManager.CurrentProcess.ProcessId;

            foreach (Entities.SchedulerTask item in tasks)
            {
                if (item.IsEnabled && dataManager.TryLockTask(item.TaskId, currentRuntimeProcessId, runningRuntimeProcessesIds, _acceptableTaskStatuses))
                {
                    Task task = new Task(() =>
                        {
                            try
                            {
                                if (item.NextRunTime == null)
                                {
                                    item.NextRunTime = item.TaskTrigger.CalculateNextTimeToRun();
                                }
                                else
                                {
                                    Dictionary<string, object> evaluatedExpressions = item.TaskTrigger.EvaluateExpressions(item);

                                    item.Status = Entities.SchedulerTaskStatus.InProgress;
                                    dataManager.UpdateTask(item);

                                    item.TaskAction.Execute(item, evaluatedExpressions);
                                    item.Status = Entities.SchedulerTaskStatus.Completed;

                                    item.NextRunTime = item.TaskTrigger.CalculateNextTimeToRun();
                                    item.LastRunTime = DateTime.Now;
                                }
                            }
                            catch(Exception ex)
                            {
                                item.Status = Entities.SchedulerTaskStatus.Failed;
                                LoggerFactory.GetExceptionLogger().WriteException(ex);
                            }
                            finally
                            {
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
