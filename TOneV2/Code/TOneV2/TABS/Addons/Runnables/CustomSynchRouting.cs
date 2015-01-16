using System.Collections.Generic;

namespace TABS.Addons.Runnables
{
    public class CustomSynchRouting
    {
        internal log4net.ILog log = log4net.LogManager.GetLogger("CustomSynchRouting");

        protected List<TABS.Switch> Switches { get; set; }

        public CustomSynchRouting(int[] SwitchIds)
        {
            Switches = new List<Switch>();
            foreach (var item in SwitchIds)
                Switches.Add(Switch.All[item]);
        }


        public void SynchRoutes()
        {
            TABS.Components.Engine.SynchRoutes(log, RouteSynchType.Full, Switches);
        }
    }
}
