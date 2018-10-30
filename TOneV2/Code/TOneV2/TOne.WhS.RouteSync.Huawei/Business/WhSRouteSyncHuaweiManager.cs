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
        string _switchId;
        IWhSRouteSyncHuaweiDataManager _dataManager;

        public WhSRouteSyncHuaweiManager(string switchId)
        {
            _switchId = switchId;

            _dataManager = RouteSyncHuaweiDataManagerFactory.GetDataManager<IWhSRouteSyncHuaweiDataManager>();
            _dataManager.SwitchId = switchId;
        }

        public void Initialize()
        {
            _dataManager.Initialize(new WhSRouteSyncHuaweiInitializeContext());
        }
    }
}
