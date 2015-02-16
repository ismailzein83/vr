using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class RouteOverrideAction : BaseRouteRuleAction
    {
        public List<OverrideOption> Options { get; set; }

        public override string ActionDisplayName
        {
            get { return "Route Override"; }
        }
    }

    public class OverrideOption
    {
        public string SupplierId { get; set; }

        public Int16? Percentage { get; set; }
    }
}
