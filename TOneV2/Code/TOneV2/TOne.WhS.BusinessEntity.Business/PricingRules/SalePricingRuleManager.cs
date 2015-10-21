using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
namespace TOne.WhS.BusinessEntity.Business
{
    public class SalePricingRuleManager : BasePricingRuleManager<SalePricingRule, SalePricingRuleDetail, SalePricingRulesInput>
    {
        protected override IEnumerable<Vanrise.Rules.BaseRuleStructureBehavior> GetBehaviors()
        {
            List<Vanrise.Rules.BaseRuleStructureBehavior> behaviors = new List<Vanrise.Rules.BaseRuleStructureBehavior>();
            behaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorByCustomer());
            behaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorBySaleZone());

            return behaviors;
        }
        public Vanrise.Entities.IDataRetrievalResult<SalePricingRuleDetail> GetFilteredSalePricingRules(Vanrise.Entities.DataRetrievalInput<SalePricingRuleQuery> input)
        {
            Func<SalePricingRule, bool> filterExpression = (prod) =>
                 (input.Query.Description == null || prod.Description.ToLower().Contains(input.Query.Description.ToLower()))
                 &&
                 (input.Query.RuleTypes == null || input.Query.RuleTypes.Contains(prod.Settings.RuleType));
                 // &&
                 //(input.Query.CarrierAccountsIds == null || input.Query.CarrierAccountsIds.Contains(prod.CarrierAccountId))
                 //  &&

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, base.GetFilteredRules(filterExpression).ToBigResult(input, filterExpression, MapToDetails));
        }

        protected override SalePricingRuleDetail MapToDetails(SalePricingRule rule)
        {
            return new SalePricingRuleDetail
            {
                Entity = rule,
                RuleTypeName=rule.Settings.RuleType.ToString(),

            };
        }

        protected override PricingRuleTODTarget CreateTODTarget(SalePricingRulesInput input)
        {
            return new SalePricingRuleTODTarget
            {
                CustomerId = input.CustomerId,
                SaleZoneId = input.SaleZoneId
            };
        }

        protected override PricingRuleTariffTarget CreateTariffTarget(SalePricingRulesInput input)
        {
            return new SalePricingRuleTariffTarget
            {
                CustomerId = input.CustomerId,
                SaleZoneId = input.SaleZoneId
            };
        }

        protected override PricingRuleExtraChargeTarget CreateExtraChargeTarget(SalePricingRulesInput input)
        {
            return new SalePricingRuleExtraChargeTarget
            {
                CustomerId = input.CustomerId,
                SaleZoneId = input.SaleZoneId
            };
        }
    }
}
