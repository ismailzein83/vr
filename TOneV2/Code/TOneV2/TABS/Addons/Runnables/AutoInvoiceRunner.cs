using TABS.Components;
using System.Web.SessionState;

namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Automatic Invoice Generation", "Generate Automatic Invoice for Customers defined With Automatic Invoice Setting.")]
    public class AutoInvoiceRunner : RunnableBase, IRequiresSessionState
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("Automatic Invoice Runner");
        internal static string status;

        public static SecurityEssentials.User CurrentUser
        {
            get;
            set;
        }

        public static void SetUser(SecurityEssentials.User user)
        {
            CurrentUser = user;
        }
        

        public override void Run()
        {
            log.Info("Automatic invoice task runner has started.");
            status = string.Empty;
            Engine.IssueInvoice(CurrentUser);
            status = string.Empty;
            log.Info("Automatic invoice task runner has ended.");
        }

        public override string Status
        {
            get
            {
                if (Components.Engine.IsAutomaticInvoiceRunning)
                {
                    return "Automatic Invoice is running";
                }
                else
                    return status;
            }
        }

        public override bool Stop()
        {
            TABS.Components.Engine.StopAutomaticInvoice();
            log.Info("Automatic invoice task runner has stopped.");
            return base.Stop();
        }

        public override bool Abort()
        {
            TABS.Components.Engine._IsAutomaticInvoiceRunning = false;
            log.Info("Automatic invoice task runner is aborted.");
            return base.Abort();
        }
    }
}
