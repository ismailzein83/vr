using NP.IVSwitch.Data;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.RouteSync.IVSwitch;

namespace NP.IVSwitch.Business
{
    public class Helper
    {
        public static void SetSwitchConfig(IDataManager dataManager)
        {
            SwitchManager switchManager = new SwitchManager();
            var switches = switchManager.GetAllSwitches();
            foreach (var switchelt in switches)
            {
                BuiltInIVSwitchSWSync routeSync = (BuiltInIVSwitchSWSync)switchelt.Settings.RouteSynchronizer;
                if (routeSync != null)
                {
                    dataManager.IvSwitchSync = routeSync;
                    break;
                }
            }

        }
    }
}
