using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.LCR.Entities
{
    public class RouteBlockAction : BaseRouteRuleAction
    {
        public MultipleSelection<string> Suppliers { get; set; }

        public override string ActionDisplayName
        {
            get { return "Route Block"; }
        }
    }
}