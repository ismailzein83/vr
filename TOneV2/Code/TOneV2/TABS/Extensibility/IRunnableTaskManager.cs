using System.Collections.Generic;

namespace TABS.Extensibility
{
    public interface IRunnableTaskManager
    {
        /// <summary>
        /// Gets the Tasks managed by this manager.
        /// The keys should be (not obligatory) the names of the tasks.
        /// </summary>
        IDictionary<string, IRunnableTask> Tasks { get; }

        /// <summary>
        /// Request a stop for all tasks
        /// </summary>
        void StopAll();

        /// <summary>
        /// Request Task Controller Shutdown (stop all running and scheduled tasks).
        /// This should mainly be done on Application Stop, to stop all running threads.
        /// </summary>
        void ShutDown();

        bool ShutDownRequested { get; }
    }
}