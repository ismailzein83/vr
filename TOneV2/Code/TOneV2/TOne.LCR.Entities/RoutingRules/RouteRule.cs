using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.LCR.Entities
{
    public enum RouteRuleType { RouteRule = 1, RouteOptionRule = 2 }
    public class RouteRule
    {
        public int RouteRuleId { get; set; }

        public BaseCarrierAccountSet CarrierAccountSet { get; set; }

        public BaseCodeSet CodeSet { get; set; }

        public RouteRuleType Type { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }

        public string Reason { get; set; }

        public BaseRouteRuleActionData ActionData { get; set; }
    }
}