using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;
using System.Timers;
using System.IO;
using CallGeneratorLibrary;
using CallGeneratorLibrary.Repositories;

namespace NewCallGenService
{
    public partial class NewCallGen : ServiceBase
    {
        System.Timers.Timer GlobalTimer = new System.Timers.Timer(15000);
        public CallDistributor c = new CallDistributor();

        internal static ServiceHost myServiceHost = null;

        public NewCallGen()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                //base.RequestAdditionalTime(30000); // 1 minutes timeout for startup
                WriteToEventLog("new start Thread111:: " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());

                
                GlobalTimer.Elapsed += OnTimedEvent;
                GlobalTimer.Enabled = true;

                c.Start();
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("EXCEPTION S: " + ex.ToString());
            }
        }

        protected override void OnStop()
        {
            if (myServiceHost != null)
            {
                myServiceHost.Close();
                myServiceHost = null;
            }
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            try
            {
                WriteToEventLog("new start Thread:: " + System.Threading.Thread.CurrentThread.ManagedThreadId.ToString());

                
                WriteToEventLog("new start:: " + CallDistributor.Centers.Count);
                if (CallDistributor.Centers.Count == 0)
                {
                    WriteToEventLog("new starttttt from service.cs");
                    c.Start();
                }
                else
                {
                    DateTime dt = CallEntryRepository.TopCallEntry().StartDate.Value;

                    WriteToEventLog("dt:: " + dt.ToString());

                    TimeSpan span = new TimeSpan();
                    if (dt != null)
                        span = DateTime.Now - dt;

                    double totalMinutes = span.TotalMinutes;

                    if (totalMinutes > 20)
                    {
                        WriteToEventLog("totalMinutes:: " + totalMinutes.ToString());

                        //End all Sessions and Entries
                        List<CallSession> LstCs = new List<CallSession>();
                        LstCs = CallSessionRepository.GetCallSessions();
                        for (int i = 0; i < LstCs.Count(); i++)
                        {
                            LstCs[i].EndDate = DateTime.Now;
                            CallSessionRepository.Update(LstCs[i]);
                        }

                        List<CallEntry> LstEntriess = new List<CallEntry>();
                        LstEntriess = CallEntryRepository.GetCallEntries();

                        for (int i = 0; i < LstEntriess.Count(); i++)
                        {
                            LstEntriess[i].EndDate = DateTime.Now;
                            LstEntriess[i].IsProcessed = 0;
                            CallEntryRepository.Save(LstEntriess[i]);
                        }

                        CallDistributor.Centers.Clear();

                        WriteToEventLog("CallDistributor.Centers.Count:: " + CallDistributor.Centers.Count());

                        c.Start();
                    }
                }
            }
            catch (System.Exception ex)
            {
                WriteToEventLogEx("OnTimedEvent EXCEPTION  S: " + ex.ToString());
            }
        }

        private void WriteToEventLog(string message)
        {
            string cs = "Call Generator Service";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }

        private void WriteToEventLogEx(string message)
        {
            string cs = "Call Generator Service";
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
