using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SalePricingRuleQuery
    {
        public string Description { get; set; }
        public List<PricingRuleType> RuleTypes { get; set; }

        public List<int> CustomerIds { get; set; }
        public List<long> SaleZoneIds { get; set; }
        public DateTime? EffectiveDate { get; set; }
    }
}
