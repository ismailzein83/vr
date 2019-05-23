using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public enum RoutingApprovalType
    {
        [Description("Approve All")]
        ApproveAll = 0,
        [Description("Reject All")]
        RejectAll = 1,
    }
}