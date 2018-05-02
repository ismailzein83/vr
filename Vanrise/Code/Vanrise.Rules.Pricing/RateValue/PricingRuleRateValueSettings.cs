using System;
using System.Collections.Generic;
using Vanrise.GenericData.Entities;

namespace Vanrise.Rules.Pricing
{
    public abstract class PricingRuleRateValueSettings
    {
        public abstract Guid ConfigId { get; }

        public int CurrencyId { get; set; }

        protected abstract void Execute(IPricingRuleRateValueContext context);

        public void ApplyRateValueRule(IPricingRuleRateValueContext context)
        {
            this.Execute(context);
        }

        public abstract string GetDescription(IGenericRuleSettingsDescriptionContext context);

        public virtual Dictionary<string, object> GetSettingsValuesByName()
        {
            return null;
        }
    }
}