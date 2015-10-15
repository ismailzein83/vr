using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class PricingRuleTODBehavior
    {
        public abstract void Execute(PricingRuleTODSettings settings, PricingRuleTODTarget target);
    }
}
