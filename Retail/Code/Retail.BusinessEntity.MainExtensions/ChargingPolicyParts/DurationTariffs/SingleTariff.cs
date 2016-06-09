using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.ChargingPolicyParts.DurationTariffs
{
    public class SingleTariff : ChargingPolicyDurationTariff
    {
        public Vanrise.Rules.Pricing.PricingRuleTariffSettings TariffSettings { get; set; }

        public override void Execute(IChargingPolicyDurationTariffContext context)
        {
            if (TariffSettings == null)
                throw new ArgumentNullException("TariffSettings");
            var pricingRuleContext = Helper.CreateTariffRuleContext(context);
            this.TariffSettings.ApplyTariffRule(pricingRuleContext);
            Helper.UpdateVoiceTariffContext(context, pricingRuleContext);
        }
    }
}
