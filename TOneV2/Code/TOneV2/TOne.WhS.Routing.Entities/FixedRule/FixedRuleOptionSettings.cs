using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class FixedRuleOptionSettings
    {
        public int SupplierId { get; set; }

        public int? Percentage { get; set; }

        public List<RouteOptionFilterSettings> Filters { get; set; }
    }
}