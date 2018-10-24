using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Huawei.Data;
using TOne.WhS.RouteSync.Huawei.Entities;

namespace TOne.WhS.RouteSync.Huawei.Business
{
    public class WhSRouteSyncHuaweiManager
    {
        public void Initialize(string switchId)
        {
            IWhSRouteSyncHuaweiDataManager whSRouteSyncHuaweiDataManager = RouteSyncHuaweiDataManagerFactory.GetDataManager<IWhSRouteSyncHuaweiDataManager>();
            whSRouteSyncHuaweiDataManager.SwitchId = switchId;
            whSRouteSyncHuaweiDataManager.Initialize(new WhSRouteSyncHuaweiInitializeContext());
        }
    }
}
