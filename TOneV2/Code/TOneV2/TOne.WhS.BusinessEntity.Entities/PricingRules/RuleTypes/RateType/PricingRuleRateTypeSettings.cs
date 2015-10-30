using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class PricingRuleRateTypeSettings : PricingRuleSettings
    {
        public List<PricingRuleRateTypeItemSettings> Items { get; set; }
    }
}
