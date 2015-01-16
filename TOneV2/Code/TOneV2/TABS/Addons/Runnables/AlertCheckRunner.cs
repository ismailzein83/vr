using System;
using System.Collections.Generic;

namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Alerts Runnable", "Check for Alerts.")]
    public class AlertCheckRunner : RunnableBase
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("Check for Alerts");
        //static TABS.AlertingService.ChildServiceClient runner;
        internal static string status;

        internal static IEnumerable<Billing_CDR_Base> _billingCDRs;
        public static IEnumerable<Billing_CDR_Base> billingCDRs
        {
            get { return _billingCDRs; }
            set { _billingCDRs = value; }
        }
        /// <summary>
        /// Start Polling The Alerting service
        /// </summary>
        /// <returns></returns>
        public override void Run()
        {
            if (!TABS.Components.Engine.IsCheckForAlertsRunning)
            {
                TABS.Components.Engine.IsCheckForAlertsRunning = true;
                status = "Creating Proxy";
                TABS.AlertingService.ChildServiceClient runner = new TABS.AlertingService.ChildServiceClient();
                runner.InnerChannel.OperationTimeout = TimeSpan.FromMinutes(10);
                status = "Polling Service";
                runner.GetAlerts();
                status = "Finished Polling Service";
                try
                {
                    status = "Closing Proxy";
                    runner.Close();
                }
                catch
                { 
                }
                TABS.Components.Engine.IsCheckForAlertsRunning = false;
            }

        }

        /// <summary>
        /// Request a stop for the operation
        /// </summary>
        /// <returns></returns>
        public override bool Stop()
        {
            //try { runner.Close(); }
            //catch { }
            //runner = null;
            TABS.Components.Engine.IsCheckForAlertsRunning = false;
            return base.Stop();
        }

        public override bool Abort()
        {
            //try { runner.Close(); }
            //catch { }
            //runner = null;
            TABS.Components.Engine.IsCheckForAlertsRunning = false;
            return base.Abort();
        }

        public override string Status
        {
            get
            {

                return status;
            }
        }

    }
}
