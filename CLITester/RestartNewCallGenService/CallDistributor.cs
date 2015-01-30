using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceProcess;
using System.Diagnostics;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;

namespace RestartNewCallGenService
{
    public class CallDistributor
    {
        public void Start()
        {
            WriteToEventLog("START CCCC");
            WriteToEventLog("new start CCCC:: " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());
            StopService("NewCallGenSer", 60000);

            System.Threading.Thread.Sleep(20000);

            //End all Sessions and Entries
            List<CallSession> LstCs = new List<CallSession>();
            LstCs = CallSessionRepository.GetCallSessions();
            for (int i = 0; i < LstCs.Count(); i++)
            {
                LstCs[i].EndDate = DateTime.Now;
                CallSessionRepository.Save(LstCs[i]);
            }

            List<CallEntry> LstEntriess = new List<CallEntry>();
            LstEntriess = CallEntryRepository.GetCallEntries();

            for (int i = 0; i < LstEntriess.Count(); i++)
            {
                LstEntriess[i].EndDate = DateTime.Now;
                CallEntryRepository.Save(LstEntriess[i]);
            }

            StartService("NewCallGenSer", 60000);
        }


        public static void StartService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                WriteToEventLog("START StartService");


                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Start();
                service.WaitForStatus(ServiceControllerStatus.Running, timeout);
            }
            catch(System.Exception ex)
            {
                WriteToEventLog("START StartService" + ex.ToString());
                // ...
            }
        }

        public static void StopService(string serviceName, int timeoutMilliseconds)
        {
            ServiceController service = new ServiceController(serviceName);
            try
            {
                WriteToEventLog("START StopService");

                TimeSpan timeout = TimeSpan.FromMilliseconds(timeoutMilliseconds);

                service.Stop();
                service.WaitForStatus(ServiceControllerStatus.Stopped, timeout);
            }
            catch(System.Exception ex)
            {
                WriteToEventLog("START StopService EXX" + ex.ToString());
                // ...
            }
        }

        private static void WriteToEventLog(string message)
        {
            string cs = "Restart Call Generator Service";
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
