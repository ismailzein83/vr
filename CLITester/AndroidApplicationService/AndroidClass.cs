using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Timers;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;
using System.Threading.Tasks;
using System.Configuration;

namespace AndroidApplicationService
{
    public class AndroidClass
    {
        #region Definitions
        
        private static readonly object _syncRoot = new object();
   
        //public bool finishCall = false;
        public static int OperatorId = 0;
        // Create a timer with a five seconds interval.
        System.Timers.Timer aTimer = new System.Timers.Timer(1000);

        public RequestForCalls thRequestForCalls = new RequestForCalls();
        public GetCLIs thGetCLIs = new GetCLIs();

        #endregion

        public void Start()
        {
            WriteToEventLog("Start Android Service: " +DateTime.Now);

            aTimer.Elapsed += OnTimedEvent;
            aTimer.Enabled = true;
            aTimer.Interval = 1000;
            aTimer.Start();
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                thRequestForCalls.Start();
                thGetCLIs.Start();
            }
            catch (System.Exception ex)
            {
                WriteToEventLog(ex.ToString());
                Logger.LogException(ex);
            }
        }

        private void WriteToEventLog(string message)
        {
            string cs = "Android Service";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }
    }
}
