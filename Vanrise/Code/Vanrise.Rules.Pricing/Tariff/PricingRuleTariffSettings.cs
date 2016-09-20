using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Rules.Pricing
{
    public abstract class PricingRuleTariffSettings
    {
        public virtual Guid ConfigId { get; set; }
        
        public int CurrencyId { get; set; }

        protected abstract void Execute(IPricingRuleTariffContext context);

        public void ApplyTariffRule(IPricingRuleTariffContext context)
        {
            this.Execute(context);
        }

        public abstract string GetDescription();
    }
}
