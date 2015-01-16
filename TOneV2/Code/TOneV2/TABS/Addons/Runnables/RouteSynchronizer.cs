
namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Route Synchronizer", "Runs a synchronization of routes on defined switches by calling the switch manager for each.")]
    public class RouteSynchronizer : RunnableBase
    {
        static log4net.ILog log = log4net.LogManager.GetLogger("TABS.RouteSynchronizer");

        public override bool IsRunning
        {
            get
            {
                return TABS.Components.Engine.IsRouteOperationRunning;
            }
        }

        public override bool IsStopRequested
        {
            get
            {
                return TABS.Components.Engine.IsRouteOperationStopRequested;
            }
        }

        /// <summary>
        /// Runs an Update of the Routes for the system Switches
        /// </summary>
        /// <returns>True on Operation Success, false otherwise</returns>
        public override void Run()
        {
            if (!TABS.Components.Engine.LastRouteBuild.HasValue)
            {
                log.Info("Cannot run Route Synchronizer when no route build has been performed yet");
            }
            else
            {
                bool fullUpdate = (
                    // Not synched before
                                    !TABS.Components.Engine.LastRouteSynch.HasValue
                                    ||
                    // Last build is newer than last synch
                                    (TABS.Components.Engine.LastRouteBuild.Value > TABS.Components.Engine.LastRouteSynch.Value)
                                    );
                Run(fullUpdate ? RouteSynchType.Full : RouteSynchType.Differential);
            }
        }

        /// <summary>
        /// Performs an update of the Routes on the system Switches.
        /// </summary>
        /// <param name="fullUpdate">True to perform a "Full Update" (all routes are uploaded and processed), False to perform a differential update (changed routes since last rebuild)</param>
        /// <returns>True on Operation Success, false otherwise</returns>
        public bool Run(RouteSynchType updateType)
        {
            bool success = Components.Engine.SynchRoutes(log, updateType);


            return success;
        }

        public override string Status { get { return (this.IsRunning) ? TABS.Components.Engine._RouteOpertationStatus : ""; } }
    }
}
