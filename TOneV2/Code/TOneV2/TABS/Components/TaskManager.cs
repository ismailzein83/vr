using System;
using System.Collections.Generic;
using System.Linq;

namespace TABS.Components
{
    /// <summary>
    /// The Task Manager (and Scheduler) for the System
    /// </summary>
    public class TaskManager : Extensibility.IRunnableTaskManager
    {
        #region static

        static log4net.ILog log = log4net.LogManager.GetLogger(typeof(TaskManager));

        /// <summary>
        /// A helper class to sort tasks by state...
        /// </summary>
        protected class TaskLister : IComparer<Extensibility.IRunnableTask>
        {
            #region IComparer<IRunnableTask> Members

            /*
            public int Compare(TABS.Extensibility.IRunnableTask x, TABS.Extensibility.IRunnableTask y)
            {
                int xRank = 0;
                int yRank = 0;

                // Running?
                xRank += x.IsRunning ? 10000 : 0;
                yRank += y.IsRunning ? 10000 : 0;

                // Enabled?
                xRank += x.IsEnabled ? 1000 : 0;
                yRank += y.IsEnabled ? 1000 : 0;

                // Last Run 
                if (x.LastRun.HasValue && y.LastRun.HasValue)
                {
                    int factor = x.LastRun.Value.CompareTo(y.LastRun.Value);
                    xRank += 50 + factor * 50;
                    yRank += 50 - factor * 50;
                }
                
                return yRank.CompareTo(xRank);
            }
             */

            public int Compare(TABS.Extensibility.IRunnableTask x, TABS.Extensibility.IRunnableTask y)
            {
                // if (y.IsEnabled == x.IsEnabled) 
                return x.Name.CompareTo(y.Name);
                // else
                //    return (y.IsEnabled.CompareTo(x.IsEnabled)); 
            }

            public static List<Extensibility.IRunnableTask> Sort(IEnumerable<Extensibility.IRunnableTask> collection)
            {
                List<Extensibility.IRunnableTask> tasks = new List<TABS.Extensibility.IRunnableTask>(collection);
                tasks.Sort(new TaskLister());
                return tasks;
            }

            #endregion
        }

        /// <summary>
        /// Returns the currrent tasks sorted by their state
        /// </summary>
        public static IEnumerable<Extensibility.IRunnableTask> TaskList
        {
            get
            {
                return TaskLister.Sort(TaskManager.Instance.Tasks.Values);
            }
        }

        public static int TaskCount
        {
            get
            {
                return TaskManager.Instance.Tasks.Count;
            }
        }

        public static void Run(TABS.Extensibility.IRunnableTask task, bool throwExceptionIfNotRunnable)
        {

            var groups = task.Groups;
            var runnable = (groups.Count == 0 || groups.Intersect(TaskManager.Instance.Tasks.Where(t => t.Value.IsRunning).SelectMany(t => t.Value.ExclusiveGroups)).Count() == 0);
            if (!runnable)
            {
                if (throwExceptionIfNotRunnable)
                {
                    var message = string.Format("Cannot run {0} ({1}) while another exclusive in the same group is running", task.Name, task.GroupingExpression);

                    log.Info(message);

                    throw new InvalidOperationException(message);

                }
                else
                    return;
            }

            System.Threading.ThreadStart threadStart = new System.Threading.ThreadStart(task.Run);
            System.Threading.Thread thread = new System.Threading.Thread(threadStart);
            task.Thread = thread;
            thread.Start();
        }

        public TimeSpan SchedulerSleepTime { get; set; }

        protected void Scheduler()
        {
            SchedulerSleepTime = SystemConfiguration.KnownParameters[KnownSystemParameter.sys_TaskResolutionTime].TimeSpanValue.Value;
            while (!ShutDownRequested)
            {
                if (IsSchedulerEnabled)
                    foreach (TABS.Extensibility.IRunnableTask task in this.Tasks.Values)
                        if (task.IsEnabled && task.ScheduleType.ToString().StartsWith("Scheduled_"))
                            if (!task.IsRunning)
                                // If the runnable was expected to run
                                if (task.NextRun.HasValue && DateTime.Now > task.NextRun)
                                {
                                    Run(task, false);
                                    System.Threading.Thread.Sleep(10); // Give milliseconds for the task to launch (fix for grouping)
                                }
                System.Threading.Thread.Sleep(SchedulerSleepTime);
            }
        }

        protected static bool? _IsSchedulerEnabled;

        public static bool IsSchedulerEnabled
        {
            get
            {
                if (_IsSchedulerEnabled == null)
                {
                    _IsSchedulerEnabled = "1".Equals(System.Configuration.ConfigurationManager.AppSettings["TABS.Components.TaskManager.EnableScheduler"]);
                }
                return _IsSchedulerEnabled.Value;
            }
            set
            {
                _IsSchedulerEnabled = value;
            }
        }

        protected static TaskManager _Instance = new TaskManager();

        /// <summary>
        /// The singleton for Managing all system's Runnable Tasks
        /// </summary>
        public static TaskManager Instance { get { return _Instance; } }

        #endregion static

        #region Members
        Dictionary<string, Extensibility.IRunnableTask> _Tasks;
        #endregion Members

        /// <summary>
        /// Create a Task manager
        /// </summary>
        protected TaskManager()
        {
            lock (typeof(TaskManager))
            {
                _Tasks = new Dictionary<string, TABS.Extensibility.IRunnableTask>();

                IList<PersistedRunnableTask> persistedTasks = ObjectAssembler.GetList<PersistedRunnableTask>();

                foreach (PersistedRunnableTask persistedTask in persistedTasks)
                {
                    Add(persistedTask);
                    if (persistedTask.ScheduleType == RunnableTaskSchedule.Startup)
                    {
                        try
                        {
                            Run(persistedTask, false);
                        }
                        catch (Exception ex)
                        {
                            log.Error(string.Format("Error running the startup Task {0}", persistedTask.Name), ex);
                        }
                    }
                }

                System.Threading.ThreadStart schedulerStart = new System.Threading.ThreadStart(Scheduler);
                System.Threading.Thread schedulerThread = new System.Threading.Thread(schedulerStart);
                schedulerThread.Start();

                // Start the Periodic alerts check
                //System.Threading.ThreadStart starter = new System.Threading.ThreadStart(Engine.PeriodicAlertsChecker);
                //System.Threading.Thread thread = new System.Threading.Thread(starter);
                //thread.Start();
            }
        }

        protected static DateTime? GetDatePreferredTime(DateTime date, TimeSpan? time)
        {
            if (time.HasValue)
                return date.Date.Add(time.Value);
            else
                return date;
        }

        public static DateTime? CalculateNextRun(Extensibility.IRunnableTask runnableTask, DateTime lastRun)
        {
            switch (runnableTask.ScheduleType)
            {
                case RunnableTaskSchedule.None:
                    return null;
                case RunnableTaskSchedule.Startup:
                    return DateTime.Now;
                case RunnableTaskSchedule.Scheduled_Once:
                    return runnableTask.NextRun;
                case RunnableTaskSchedule.Scheduled_Hourly:
                    if (runnableTask.TimeSpan.HasValue)
                    {
                        DateTime nextHour = lastRun.AddHours(1).Subtract(new TimeSpan(0, 0, lastRun.Minute, lastRun.Second, lastRun.Millisecond));
                        //ignore hours
                        TimeSpan fixedSpan = new TimeSpan
                            (0, runnableTask.TimeSpan.Value.Minutes, runnableTask.TimeSpan.Value.Seconds);
                        return nextHour.Add(fixedSpan);
                    }
                    else
                    {
                        return lastRun.AddHours(1);
                    }
                case RunnableTaskSchedule.Scheduled_Daily:
                    return GetDatePreferredTime(lastRun.AddDays(1), runnableTask.TimeSpan);
                case RunnableTaskSchedule.Scheduled_Weekly:
                    return GetDatePreferredTime(lastRun.AddDays(7), runnableTask.TimeSpan);
                case RunnableTaskSchedule.Scheduled_Monthly:
                    return GetDatePreferredTime(lastRun.AddMonths(1), runnableTask.TimeSpan);
                case RunnableTaskSchedule.Scheduled_Custom:
                    return lastRun.Add(runnableTask.TimeSpan.Value);
                default:
                    return null;
            }
        }

        public static DateTime? CalculateNextRun(Extensibility.IRunnableTask runnableTask)
        {
            DateTime lastRun = runnableTask.LastRun.GetValueOrDefault(DateTime.Now);
            return CalculateNextRun(runnableTask, lastRun);
        }

        /// <summary>
        /// Add a runnable task to this task manager.
        /// </summary>
        /// <param name="runnableTask"></param>
        /// <returns></returns>
        public bool Add(Extensibility.IRunnableTask runnableTask)
        {
            if (!Tasks.ContainsKey(runnableTask.Name))
            {
                // Add to tasks collection
                Tasks.Add(runnableTask.Name, runnableTask);

                // Set this as the manager for this task
                runnableTask.Manager = this;

                return true;
            }
            else
                return false;
        }

        #region IRunnableTaskManager Members

        /// <summary>
        /// Get the list of Tasks managed by this manager.
        /// </summary>
        public IDictionary<string, Extensibility.IRunnableTask> Tasks
        {
            get { return _Tasks; }
        }

        /// <summary>
        /// Request a stop for all tasks
        /// </summary>
        public void StopAll()
        {
            foreach (Extensibility.IRunnableTask task in Tasks.Values)
                task.Stop();
        }

        protected bool _ShutDownRequested = false;

        public bool ShutDownRequested { get { return _ShutDownRequested; } }

        public void ShutDown()
        {
            _ShutDownRequested = true;
        }

        #endregion

        /// <summary>
        /// Remove a task (knowing its name)
        /// </summary>
        /// <param name="taskName"></param>
        public bool Remove(string taskName)
        {
            if (Tasks.ContainsKey(taskName))
            {
                TABS.Extensibility.IRunnableTask task = Tasks[taskName];
                Tasks.Remove(taskName);
                if (task.PossessesOwnThread) task.Thread.Abort();
                return true;
            }
            else
                return false;
        }

        /// <summary>
        /// Locate the task having the runnable of a given type, and run it in the executing thread 
        /// </summary>
        /// <param name="runnableType">The Known Runnable Class (Type) to look for</param>
        public static void LocateAndRun(Type runnableType)
        {
            foreach (Extensibility.IRunnableTask runnable in TaskManager.TaskList)
                if (runnable is PersistedRunnableTask)
                {
                    PersistedRunnableTask task = (PersistedRunnableTask)runnable;
                    if (task.RunType == RunnableTaskType.KnownIRunnableClass && task.KnownIRunnableClass == runnableType.FullName)
                        task.Run();
                }
        }
    }
}