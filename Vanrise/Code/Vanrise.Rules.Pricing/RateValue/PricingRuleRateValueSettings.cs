using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.GenericData.Entities;

namespace Vanrise.Rules.Pricing
{
    public abstract class PricingRuleRateValueSettings
    {
        public virtual Guid ConfigId { get; set; }

        protected abstract void Execute(IPricingRuleRateValueContext context);

        public void ApplyRateValueRule(IPricingRuleRateValueContext context)
        {
            this.Execute(context);
        }

        public abstract string GetDescription(IGenericRuleSettingsDescriptionContext context);
    }
}
