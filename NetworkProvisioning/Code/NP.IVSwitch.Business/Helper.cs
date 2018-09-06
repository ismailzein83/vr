using NP.IVSwitch.Data;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.RouteSync.IVSwitch;

namespace NP.IVSwitch.Business
{
    public class Helper
    {
        public static void SetSwitchConfig(IDataManager dataManager)
        {
            var tempSwitch = GetSwitch();
            if (tempSwitch != null)
                dataManager.IvSwitchSync = (BuiltInIVSwitchSWSync)tempSwitch.Settings.RouteSynchronizer;
        }
        public static Switch GetSwitch()
        {
            SwitchManager switchManager = new SwitchManager();
            var switches = switchManager.GetAllSwitches();
            foreach (var switchelt in switches)
            {
                if (switchelt.Settings == null) continue;

                BuiltInIVSwitchSWSync routeSync = switchelt.Settings.RouteSynchronizer as BuiltInIVSwitchSWSync;
                if (routeSync != null)
                {
                    return switchelt;
                }
            }
            return null;
        }

        public static BuiltInIVSwitchSWSync GetIvSwitchSync()
        {
            var tempSwitch = GetSwitch();
            if (tempSwitch != null)
               return (BuiltInIVSwitchSWSync)tempSwitch.Settings.RouteSynchronizer;
            return null;
        }
    }
}
