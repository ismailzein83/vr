using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Data;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Entities;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.Business
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
