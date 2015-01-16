using System;

namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Route Affector (ToD)", "Updates the Routes and Route Options from effective ToD Considerations.")]
    public class RouteAffector_ToD : RouteAffectorBase
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("TABS.RouteAffector.ToD");

        public override void Run()
        {
            log.Info("Route Affector (ToD) requested to Run");

            if (IsRunning)
            {
                log.Info("Route Operation Already Running. Aborting Request.");
                return;
            }

            log.Info("Route Affector (ToD) Started");

            _LastRun = DateTime.Now;

            bool success = TABS.Components.Engine.ExecuteRoutingOperation(TABS.Components.Engine.RoutingOperation.Update_From_ToD, log);

            // Stopped Running
            _LastRunDuration = DateTime.Now.Subtract(_LastRun.Value);

            log.Info("Route Affector (ToD) Finished");
            IsLastRunSuccessful = true;
        }

        public override string Status
        {
            get { return string.Empty; }
        }
    }
}