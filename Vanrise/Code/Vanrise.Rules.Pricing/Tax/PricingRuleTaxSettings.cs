using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Rules.Pricing
{
    public abstract class PricingRuleTaxSettings
    {
        public abstract Guid ConfigId { get; }

        protected abstract void Execute(IPricingRuleTaxContext context);

        public void ApplyTaxRule(IPricingRuleTaxContext context)
        {
            this.Execute(context);
        }

        public abstract string GetDescription(IGenericRuleSettingsDescriptionContext context);
    }
}
