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
using Vanrise.Fzero.Services.WcfServiceLibrary;
using System.Timers;
using System.IO;

namespace Vanrise.Fzero.Services.WCF
{
    public partial class FMSService : ServiceBase
    {
        internal static ServiceHost myServiceHost = null; 

        public FMSService()
        {
            InitializeComponent();
        }
        private void ErrorLog(string message)
        {
            string cs = "FMS Service";
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
                if (myServiceHost != null)
                {
                    myServiceHost.Close();
                }
                myServiceHost = new ServiceHost(typeof(Vanrise.Fzero.Services.WcfServiceLibrary.FzeroService));
                myServiceHost.Open();
            }
            catch(Exception ex)
            {
                ErrorLog("OnStart: " + ex.ToString());
                ErrorLog("OnStart: " + ex.ToString());
                ErrorLog("OnStart: " + ex.ToString());
               
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
