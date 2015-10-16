using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Routing.Entities
{
    public class Route
    {
        public int CustomerId { get; set; }

        public string Code { get; set; }

        public long SaleZoneId { get; set; }

        public Decimal Rate { get; set; }

        public bool IsBlocked { get; set; }

        public int ExecutedRuleId { get; set; }

        public List<RouteOption> Options { get; set; }
    }
}
