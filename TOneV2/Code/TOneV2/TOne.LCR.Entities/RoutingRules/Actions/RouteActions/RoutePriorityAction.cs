using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class RoutePriorityAction : BaseRouteRuleAction
    {
        public List<PriorityOption> Options { get; set; }

        public override string ActionDisplayName
        {
            get { return "Route Priority"; }
        }
    }

    public class PriorityOption
    {
        public string SupplierId { get; set; }

        public int Order { get; set; }

        public bool Force { get; set; }
    }
}
