using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Entities;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Data;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.Business
{
    public class RouteSucceededManager
    {
        string _switchId;
        IRouteSucceededDataManager _dataManager;

        public RouteSucceededManager(string switchId)
        {
            _switchId = switchId;

            _dataManager = RouteSyncHuaweiDataManagerFactory.GetDataManager<IRouteSucceededDataManager>();
            _dataManager.SwitchId = switchId;
        }

        public void SaveRoutesSucceededToDB(Dictionary<int, List<HuaweiRouteWithCommands>> routesWithCommandsByRSSC)
        {
            _dataManager.SaveRoutesSucceededToDB(routesWithCommandsByRSSC);
        }
    }
}