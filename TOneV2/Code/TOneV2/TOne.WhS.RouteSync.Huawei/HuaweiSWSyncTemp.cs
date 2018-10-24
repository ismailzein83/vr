using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;
using TOne.WhS.RouteSync.Huawei.Entities;
using TOne.WhS.RouteSync.Huawei.Business;
using Vanrise.Common;

namespace TOne.WhS.RouteSync.Huawei
{
    public partial class HuaweiSWSync : SwitchRouteSynchronizer
    {
        public override bool IsSwitchRouteSynchronizerValid(IIsSwitchRouteSynchronizerValidContext context)
        {
            throw new NotImplementedException();
        }
    }
}