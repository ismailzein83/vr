using System;
using System.Threading;
namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Memory Trace", "Trace Memory concumption of the main TOne process")]
    public class MemoryTraceRunner : RunnableBase
    {
        bool _IsMemoryTracetRunning = false;
        static DateTime startrunningIn = DateTime.Now;
        internal static string status;
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(MemoryTraceRunner));

        /// <summary>
        /// Start the CDR Import (Get from switches and save to database)
        /// </summary>
        /// <returns></returns>
        public override void Run()
        {
            status = string.Empty;
            TotalMemoryStart("T.One - Memory Consumption Trace:");
            status = string.Empty;
            _IsMemoryTracetRunning = true;
            startrunningIn = DateTime.Now; ;
        }

        public static void TotalMemoryStart(string message)
        {
            long GC_MemoryStart = 0;
            Thread.MemoryBarrier();
            GC_MemoryStart = System.GC.GetTotalMemory(true);
            log.InfoFormat("{0} {1}KB /{2}MB", message, GC_MemoryStart / 1024, GC_MemoryStart / 1048576);
        }
        /// <summary>
        /// Request a stop for the operation
        /// </summary>
        /// <returns></returns>
        public override bool Stop()
        {
            status = "Stop :" + DateTime.Now.ToString("dd-MM-yyyy hh:mm:ss");
            return base.Stop();
        }

        public override bool Abort()
        {
            _IsMemoryTracetRunning = false;
            return base.Abort();
        }

        public override string Status
        {
            get 
            {
                if (_IsMemoryTracetRunning)
                    return "Running from: " + startrunningIn.ToString();
                else
                    return status;                
            }
        }

    }
}