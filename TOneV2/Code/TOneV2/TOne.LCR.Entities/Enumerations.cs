using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public enum RouteDetailFilterOrder
    {
        CustomerId = 0,
        Code = 1,
        Zone = 2,
        Rate = 3,
        ServiceFlag = 4
    }

    public enum RouteRuleActionType
    {
        Priority = 0,
        Override = 1,
        Block = 2
    }
}
