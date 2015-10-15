using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class PricingRuleTariffBehavior
    {
        public abstract void Execute(PricingRuleTariffSettings settings, PricingRuleTariffTarget target);
    }
}
