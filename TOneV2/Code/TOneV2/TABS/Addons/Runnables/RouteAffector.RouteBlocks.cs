using System;

namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Route Affector (Route Blocks)", "Updates the Routes and Route Options from effective Route Blocks.")]
    public class RouteAffector_RouteBlocks : RouteAffectorBase
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("TABS.RouteAffector.RouteBlocks");

        public override void Run()
        {
            log.Info("Route Affector (Route Blocks) requested to Run");

            if (IsRunning)
            {
                log.Info("Route Operation Already Running. Aborting Request.");
                return;
            }

            log.Info("Route Affector (Route Blocks) Started");

            _LastRun = DateTime.Now;

            bool success = TABS.Components.Engine.ExecuteRoutingOperation(TABS.Components.Engine.RoutingOperation.Update_From_Route_Blocks, log);

            // Stopped Running
            _LastRunDuration = DateTime.Now.Subtract(_LastRun.Value);

            log.Info("Route Affector (Route Blocks) Finished");
            IsLastRunSuccessful = true;
        }

        public override string Status
        {
            get { return string.Empty; }
        }
    }
}
