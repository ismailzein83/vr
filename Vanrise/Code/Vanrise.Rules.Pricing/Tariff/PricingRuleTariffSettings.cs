using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Rules.Pricing
{
    public abstract class PricingRuleTariffSettings
    {
        public abstract Guid ConfigId { get; }

        public int CurrencyId { get; set; }

        protected abstract void Execute(IPricingRuleTariffContext context);

        public void ApplyTariffRule(IPricingRuleTariffContext context)
        {
            this.Execute(context);
        }

        public abstract string GetDescription();
        public abstract string GetPricingDescription();

        public virtual Dictionary<string, object> GetSettingsValuesByName()
        {
            return null;
        }
    }
}
