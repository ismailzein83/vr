using System;
using System.Diagnostics;
using System.ServiceModel;
using System.ServiceProcess;
using System.Timers;

namespace WCFServiceMonty
{
    public partial class MontyService : ServiceBase
    {
        //System.Timers.Timer GlobalTimer = new System.Timers.Timer(60000);
        internal static ServiceHost myServiceHost = null;

        public MontyService()
        {
            InitializeComponent();
        }
        private void WriteToEventLog(string message)
        {
            string cs = "Monty Service";
            EventLog elog = new EventLog();
            if (!EventLog.SourceExists(cs))
            {
                EventLog.CreateEventSource(cs, cs);
            }
            elog.Source = cs;
            elog.EnableRaisingEvents = true;
            elog.WriteEntry(message);
        }

        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
        }

        protected override void OnStart(string[] args)
        {

            try
            {
                //GlobalTimer.Elapsed += OnTimedEvent;
                //GlobalTimer.Enabled = true;

                //WriteToEventLog("1 Monty");

                base.RequestAdditionalTime(60000); // 10 minutes timeout for startup

                //WriteToEventLog("2 Monty");

                Debugger.Launch(); // launch and attach debugger
                CallDistributor c = new CallDistributor();

                //WriteToEventLog("3 Monty");

                c.Start();

                //WriteToEventLog("4 Monty");


            }
            catch(Exception ex)
            {
                WriteToEventLog(ex.ToString());
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

        

    }
}
