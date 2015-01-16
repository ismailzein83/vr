
namespace TABS.Addons.Runnables
{
    /// <summary>
    /// Route Affectors Base Class. All routing (build and update) functionality should be manipulated here. 
    /// </summary>
    public abstract class RouteAffectorBase : RunnableBase
    {
        public override bool IsRunning
        {
            get
            {
                return TABS.Components.Engine.IsRouteOperationRunning;
            }
            protected set
            {
                base.IsRunning = value;
                TABS.Components.Engine.IsRouteOperationRunning = value;
            }
        }

        public override bool IsStopRequested
        {
            get
            {
                return TABS.Components.Engine.IsRouteOperationStopRequested;
            }
        }

        public override bool Stop()
        {
            base.Stop();
            return TABS.Components.Engine.StopRouteBuild();
        }
    }
}
