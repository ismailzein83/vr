using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Huawei.Data;
using TOne.WhS.RouteSync.Huawei.Entities;

namespace TOne.WhS.RouteSync.Huawei.Business
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

        public void SaveRoutesSucceededToDB(Dictionary<string, List<HuaweiRouteWithCommands>> routesWithCommandsByRSSN)
        {
            _dataManager.SaveRoutesSucceededToDB(routesWithCommandsByRSSN);
        }
    }
}