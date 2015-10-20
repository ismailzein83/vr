using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class PricingRuleTODSettings : PricingRuleSettings
    {
        public int ConfigId { get; set; }

        public abstract void Execute(IPricingRuleTODContext context, PricingRuleTODTarget target);
    }
}
