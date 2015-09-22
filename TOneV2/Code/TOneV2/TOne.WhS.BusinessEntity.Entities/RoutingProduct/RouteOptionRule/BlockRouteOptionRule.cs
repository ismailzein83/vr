using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class BlockRouteOptionRule : IRouteCriteria
    {
        public RouteCriteria RouteCriteria { get; set; }

        public int Type { get; set; }

        public BlockRouteOptionRuleSettings Settings { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }

        public string Reason { get; set; }
    }
}
