using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class RoutingRuleActionType
    {
        public int RoutingRuleActionTypeId { get; set; }

        public Type Type { get; set; }

        public int Priority { get; set; }
    }
}
