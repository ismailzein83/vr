using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.ChargingPolicyParts.RateValues
{
    public class GenericRuleRateValue : ChargingPolicyRateValue
    {
        public List<Vanrise.GenericData.Pricing.RateValueRule> RateValueRules { get; set; }

        public override void Execute(IChargingPolicyRateValueContext context)
        {
            var ruleTree = Helper.GetRuleTree(context.PricingEntity, context.PricingEntityId, context.ServiceTypeId, base.PartTypeExtensionName, context.ChargingPolicyPartSettings as BaseChargingPolicyPartRuleDefinition, this.RateValueRules);
            var pricingRuleContext = Helper.CreateRateValueRuleContext(context);
            var ruleManager = new Vanrise.GenericData.Pricing.RateValueRuleManager();
            ruleManager.ApplyRateValueRule(pricingRuleContext, ruleTree, context.RuleTarget);
            Helper.UpdateVoiceRateValueContext(context, pricingRuleContext);
        }

        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
    }
}
