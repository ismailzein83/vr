using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.Entities;

namespace TOne.LCR.Entities
{
    public abstract class BaseRouteRule
    {
        public int RouteRuleId { get; set; }

        public BaseCarrierAccountSet CarrierAccountSet { get; set; }

        public BaseCodeSet CodeSet { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }

        public string Reason { get; set; }
    }
}