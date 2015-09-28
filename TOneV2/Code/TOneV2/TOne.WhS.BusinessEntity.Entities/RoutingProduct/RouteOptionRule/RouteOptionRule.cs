using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class RouteOptionRule : IRouteCriteria
    {
        public RouteCriteria RouteCriteria { get; set; }

        public int Type { get; set; }

        public RouteOptionRuleSettings Settings { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }

        public string Reason { get; set; }
    }
}
