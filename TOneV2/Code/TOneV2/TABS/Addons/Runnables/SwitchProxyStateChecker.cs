using TABS.Addons.Utilities.ProxyCommon;

namespace TABS.Addons.Runnables
{
    [NamedAddon("T.One - Switch Proxy State Checker", "Checks the current state of the switch proxies if available.")]
    class SwitchProxyStateChecker : RunnableBase
    {
        private static log4net.ILog log = log4net.LogManager.GetLogger(typeof(RunnableBase));
        public override void Run()
        {
            foreach (Switch s in TABS.Switch.All.Values)
            {                
                s.ProxyStates = s.Enable_Routing ? s.SwitchManager.GetProxyStates(s) : ProxyState.NotAvailable.ToList();
                foreach (ProxyState state in s.ProxyStates)
                {
                    if (!string.IsNullOrEmpty(state.Error))
                        log.ErrorFormat("Error in {0} Proxy: {1}", s.Name, state.Error);
                    else
                        log.InfoFormat("Proxy {0} Status: {1}", s.Name, state.Status);
                }
                
            }
        }
        public override string Status
        {
            get { return string.Empty; }
        }
    }
}
