using System;

namespace TABS.Extensibility
{
    public interface IRunnable
    {
        /// <summary>
        /// Run and return success status
        /// </summary>
        /// <returns>True on Success</returns> 
        void Run();

        /// <summary>
        /// Request a stop while running
        /// </summary>
        /// <returns></returns>
        bool Stop();

        /// <summary>
        /// Request an abort while running
        /// </summary>
        /// <returns></returns>
        bool Abort();

        /// <summary>
        /// Indicate whether this is running
        /// </summary>
        bool IsRunning { get; }
        DateTime? LastRun { get; }
        TimeSpan? LastRunDuration { get; }
        bool IsStopRequested { get; }

        /// <summary>
        /// Get a verbal status for this runnable
        /// </summary>
        string Status { get; }

        /// <summary>
        /// Get/Set the last exception if any
        /// </summary>
        Exception Exception { get; set; }

        /// <summary>
        /// Gets whether this runnable was successfully run last time
        /// </summary>
        bool? IsLastRunSuccessful { get; set; }
    }
}
