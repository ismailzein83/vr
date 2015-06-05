using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace VoIPSwitchService
{
    public partial class VoIPSwitchService : ServiceBase
    {
        internal static ServiceHost myServiceHost = null;

        public VoIPSwitchService()
        {
            InitializeComponent();
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

        protected override void OnStart(string[] args)
        {
            try
            {
                StartService c = new StartService();
                c.Start();
            }
            catch (Exception ex)
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
