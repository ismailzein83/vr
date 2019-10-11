using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Huawei.SoftX3000.Entities;

namespace TOne.WhS.RouteSync.Huawei.SoftX3000.Data
{
    public interface IWhSRouteSyncHuaweiDataManager : IDataManager
    {
        string SwitchId { get; set; }

        void Initialize(IWhSRouteSyncHuaweiInitializeContext context);
    }
}
