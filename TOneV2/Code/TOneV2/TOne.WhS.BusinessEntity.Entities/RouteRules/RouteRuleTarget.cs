using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RouteRuleTarget
    {
        public bool IsBlock { get; set; }

        public List<RouteOptionRuleTarget> Options { get; set; }
    }
}
