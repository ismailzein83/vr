using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class PurchasePricingRuleManager : BasePricingRuleManager<PurchasePricingRule, PurchasePricingRuleDetail, PurchasePricingRulesInput>
    {
        protected override IEnumerable<Vanrise.Rules.BaseRuleStructureBehavior> GetBehaviors()
        {
            List<Vanrise.Rules.BaseRuleStructureBehavior> behaviors = new List<Vanrise.Rules.BaseRuleStructureBehavior>();
            behaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorBySupplier());
            behaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorBySupplierZone());

            return behaviors;
        }

        protected override PurchasePricingRuleDetail MapToDetails(PurchasePricingRule rule)
        {
            throw new NotImplementedException();
        }

        protected override PricingRuleTODTarget CreateTODTarget(PurchasePricingRulesInput input)
        {
            return new PurchasePricingRuleTODTarget
            {
                SupplierId = input.SupplierId,
                SupplierZoneId = input.SupplierZoneId
            };
        }

        protected override PricingRuleTariffTarget CreateTariffTarget(PurchasePricingRulesInput input)
        {
            return new PurchasePricingRuleTariffTarget
            {
                SupplierId = input.SupplierId,
                SupplierZoneId = input.SupplierZoneId
            };
        }

        protected override PricingRuleExtraChargeTarget CreateExtraChargeTarget(PurchasePricingRulesInput input)
        {
            return new PurchasePricingRuleExtraChargeTarget
            {
                SupplierId = input.SupplierId,
                SupplierZoneId = input.SupplierZoneId
            };
        }
    }
}
