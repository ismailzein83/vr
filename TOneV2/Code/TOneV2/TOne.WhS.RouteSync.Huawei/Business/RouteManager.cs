using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Huawei.Data;
using TOne.WhS.RouteSync.Huawei.Entities;

namespace TOne.WhS.RouteSync.Huawei.Business
{
    public class RouteManager
    {
        public void Initialize(string switchId)
        {
            IRouteDataManager routeCaseDataManager = RouteSyncHuaweiDataManagerFactory.GetDataManager<IRouteDataManager>();
            routeCaseDataManager.SwitchId = switchId;
            routeCaseDataManager.Initialize(new RouteInitializeContext());
        }
    }
}
