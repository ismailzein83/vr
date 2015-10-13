using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class BasePricingRule : Vanrise.Rules.BaseRule
    {       
        public PricingRuleSettings Settings { get; set; }

        public string Description { get; set; }  
    }
}
