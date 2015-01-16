using System;
using System.Threading;
namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Memory Control", "Control Memory Peak By GC")]
    public class MemoryControlRunner : RunnableBase
    {
        bool _IsMemoryControlRunning = false;
        static DateTime startrunningIn = DateTime.Now;
        internal static string status;
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(MemoryControlRunner));

        /// <summary>
        /// Start Memory Peak Control
        /// </summary>
        /// <returns></returns>
        public override void Run()
        {
            status = string.Empty;
            MemoryControl(50);
            status = string.Empty;
            _IsMemoryControlRunning = true;
            startrunningIn = DateTime.Now; ;
        }

        public static void MemoryControl(int peak)
        {
            long GC_MemoryStart = 0;
            GC_MemoryStart = MemoryPeak();
            if (GC_MemoryStart % peak == 0 || GC_MemoryStart>=peak)
            {
                GC.Collect();
                GC.Collect();
            }
        }
        public static long MemoryPeak()
        {
            long GC_MemoryStart = 0;
            Thread.MemoryBarrier();
            GC_MemoryStart = System.GC.GetTotalMemory(false);
            log.InfoFormat("Memory Peak:{0} MB", GC_MemoryStart / 1048576);
            return GC_MemoryStart / 1048576;
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
            _IsMemoryControlRunning = false;
            return base.Abort();
        }

        public override string Status
        {
            get 
            {
                if (_IsMemoryControlRunning)
                    return "Running from: " + startrunningIn.ToString();
                else
                    return status;                
            }
        }

    }
}