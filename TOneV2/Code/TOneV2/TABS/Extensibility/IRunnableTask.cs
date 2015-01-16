using System;
using System.Collections.Generic;

namespace TABS.Extensibility
{
    public interface IRunnableTask : IDisposable, IRunnable
    {
        IRunnableTaskManager Manager { get; set; }
        string Name { get; set; }
        RunnableTaskSchedule ScheduleType { get; set; }
        TimeSpan? TimeSpan { get; }        
        bool IsEnabled { get; set; }
        bool PossessesOwnThread { get; }
        string GroupingExpression { get; }
        List<string> ExclusiveGroups { get; }
        List<string> Groups { get; }

        /// <summary>
        /// Return the running thread on this Task
        /// </summary>
        System.Threading.Thread Thread { get; set; }
        
        /// <summary>
        /// Determine the Next Expected Run if this is a scheduled task
        /// </summary>
        DateTime? NextRun { get; set; }
    }
}