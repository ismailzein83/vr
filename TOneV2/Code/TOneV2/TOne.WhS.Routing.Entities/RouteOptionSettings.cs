using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class RouteOptionSettings
    {
        public int SupplierId { get; set; }

        public Decimal? Percentage { get; set; }

        public RouteRuleOptionFilterSettings Filter { get; set; }
    }
}
