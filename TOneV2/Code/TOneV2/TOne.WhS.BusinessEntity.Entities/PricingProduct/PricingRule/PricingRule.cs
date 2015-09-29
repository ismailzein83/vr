using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class PricingRule
    {
        public PricingRuleCriteria Criteria { get; set; }

        public int TypeConfigId { get; set; }

        public PricingRuleSettings Settings { get; set; }

        public DateTime BeginEffectiveDate { get; set; }

        public DateTime? EndEffectiveDate { get; set; }

        public string Description { get; set; }
    }
}
