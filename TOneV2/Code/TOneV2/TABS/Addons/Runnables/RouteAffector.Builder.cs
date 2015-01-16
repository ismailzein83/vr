using System;

namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Route Builder", "Rebuilds the Route and Route Option Tables. Generally this task takes a considerable amount of time.")]
    public class RouteBuilder : RouteAffectorBase
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("TABS.RouteAffector.Builder");

        public override void Run()
        {
            log.Info("Route Builder requested to Run");

            if (IsRunning)
            {
                log.Info("Route Operation Already Running. Aborting Request.");

            }
            else
            {
                log.Info("Route Builder Started");

                _LastRun = DateTime.Now;

                bool success = TABS.Components.Engine.ExecuteRoutingOperation(TABS.Components.Engine.RoutingOperation.Rebuild_Routes, log);

                // Stopped Running
                _LastRunDuration = DateTime.Now.Subtract(_LastRun.Value);

                log.Info("Route Builder Finished");
                IsLastRunSuccessful = true;
            }
        }

        public override string Status
        {
            get
            {
                string opStatus = string.Empty;

                if (this.IsRunning)
                {
                    opStatus = TABS.Components.Engine._RouteOpertationStatus;

                    if ("Executing Build Routes".Equals(opStatus))
                    {
                        var msgTable = DataHelper.GetDataTable("SELECT TOP 1 * FROM SystemMessage M WHERE MessageID LIKE 'BuildRoutes%' ORDER BY [timestamp] DESC");
                        if (msgTable.Rows.Count > 0)
                            opStatus = string.Format("{0} ({1})", msgTable.Rows[0]["MessageID"], msgTable.Rows[0]["Message"]);
                    }
                }

                return opStatus;
            }
        }
    }
}