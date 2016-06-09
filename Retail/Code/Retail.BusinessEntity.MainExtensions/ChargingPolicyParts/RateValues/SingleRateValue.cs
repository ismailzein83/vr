using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.ChargingPolicyParts.RateValues
{
    public class SingleRateValue : ChargingPolicyRateValue
    {
        public Vanrise.Rules.Pricing.PricingRuleRateValueSettings RateValueSettings { get; set; }

        public override void Execute(IChargingPolicyRateValueContext context)
        {
            if (RateValueSettings == null)
                throw new ArgumentNullException("RateValueSettings");
            var pricingRuleContext = Helper.CreateRateValueRuleContext(context);
            this.RateValueSettings.ApplyRateValueRule(pricingRuleContext);
            Helper.UpdateVoiceRateValueContext(context, pricingRuleContext);
        }
    }
}
