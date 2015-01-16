using System;

namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Route Affector (Sepcial Requests)", "Updates the Routes and Route Options from Special Requests.")]
    public class RouteAffector_SepcialRequests : RouteAffectorBase
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("TABS.RouteAffector.SpecialRequests");

        public override void Run()
        {
            log.Info("Route Affector (Special Requests) requested to Run");

            if (IsRunning)
            {
                log.Info("Route Operation Already Running. Aborting Request.");
                return;
            }

            log.Info("Route Affector (Special Requests) Started");

            _LastRun = DateTime.Now;

            bool success = TABS.Components.Engine.ExecuteRoutingOperation(TABS.Components.Engine.RoutingOperation.Update_From_Special_Requests, log);

            // Stopped Running
            _LastRunDuration = DateTime.Now.Subtract(_LastRun.Value);

            log.Info("Route Affector (Special Requests) Finished");
            IsLastRunSuccessful = true;
        }

        public override string Status { get { return string.Empty; } }
    }
}
