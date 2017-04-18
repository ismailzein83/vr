using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.AccountBalance.Entities
{
    public class CustomerRoutingStatusState
    {
        public BusinessEntity.Entities.RoutingStatus OriginalRoutingStatus { get; set; }

        public Dictionary<int, SwitchCustomerBlockingInfo> BlockingInfoBySwitchId { get; set; }
    }
}
