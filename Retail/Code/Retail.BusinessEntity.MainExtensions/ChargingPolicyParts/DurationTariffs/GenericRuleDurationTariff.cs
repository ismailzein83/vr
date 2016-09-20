using Retail.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.BusinessEntity.MainExtensions.ChargingPolicyParts.DurationTariffs
{
    public class GenericRuleDurationTariff : ChargingPolicyDurationTariff
    {
        public List<Vanrise.GenericData.Pricing.TariffRule> TariffRules { get; set; }

        public override void Execute(IChargingPolicyDurationTariffContext context)
        {
            var ruleTree = Helper.GetRuleTree(context.PricingEntity, context.PricingEntityId, context.ServiceTypeId, base.PartTypeExtensionName, context.ChargingPolicyPartSettings as BaseChargingPolicyPartRuleDefinition, this.TariffRules);
            var pricingRuleContext = Helper.CreateTariffRuleContext(context);
            var ruleManager = new Vanrise.GenericData.Pricing.TariffRuleManager();
            ruleManager.ApplyTariffRule(pricingRuleContext, ruleTree, context.RuleTarget);
            Helper.UpdateVoiceTariffContext(context, pricingRuleContext);
        }

        public override Guid ConfigId
        {
            get { throw new NotImplementedException(); }
        }
    }
}
