using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.ChargingPolicyParts.RateTypes
{
    public class GenericRuleRateType : ChargingPolicyRateType
    {
        public List<Vanrise.GenericData.Pricing.RateTypeRule> RateTypeRules { get; set; }

        public override void Execute(IChargingPolicyRateTypeContext context)
        {
            var ruleTree = Helper.GetRuleTree(context.PricingEntity, context.PricingEntityId, context.ServiceTypeId, base.PartTypeExtensionName, context.ChargingPolicyPartSettings as BaseChargingPolicyPartRuleDefinition, this.RateTypeRules);
            var pricingRuleContext = Helper.CreateRateTypeRuleContext(context);
            var ruleManager = new Vanrise.GenericData.Pricing.RateTypeRuleManager();
            ruleManager.ApplyRateTypeRule(pricingRuleContext, ruleTree, context.RuleTarget);
            Helper.UpdateVoiceRateTypeContext(context, pricingRuleContext);
        }

        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
    }
}
