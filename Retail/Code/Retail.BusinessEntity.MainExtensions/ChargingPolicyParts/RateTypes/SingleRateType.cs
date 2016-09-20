using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.ChargingPolicyParts.RateTypes
{
    public class SingleRateType : ChargingPolicyRateType
    {
        public Vanrise.Rules.Pricing.PricingRuleRateTypeSettings RateTypeSettings { get; set; }

        public override void Execute(IChargingPolicyRateTypeContext context)
        {
            if (RateTypeSettings == null)
                throw new ArgumentNullException("RateTypeSettings");
            var pricingRuleContext = Helper.CreateRateTypeRuleContext(context);
            this.RateTypeSettings.ApplyRateTypeRule(pricingRuleContext);
            Helper.UpdateVoiceRateTypeContext(context, pricingRuleContext);
        }

        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
    }
}
