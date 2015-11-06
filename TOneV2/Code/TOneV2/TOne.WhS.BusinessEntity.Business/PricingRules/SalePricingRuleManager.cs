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
                 && (input.Query.RuleTypes == null || input.Query.RuleTypes.Contains(prod.Settings.RuleType))
                 && (input.Query.CustomerIds == null || this.CheckIfCustomerSettingsContains(prod, input.Query.CustomerIds))
                 && (input.Query.SaleZoneIds == null || this.CheckIfSaleZoneSettingsContains(prod, input.Query.SaleZoneIds))
                 && (input.Query.EffectiveDate == null || (prod.BeginEffectiveTime <= input.Query.EffectiveDate && (prod.EndEffectiveTime == null || prod.EndEffectiveTime >= input.Query.EffectiveDate)));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, base.GetFilteredRules(filterExpression).ToBigResult(input, filterExpression, MapToDetails));
        }
        private bool CheckIfSaleZoneSettingsContains(SalePricingRule salePricingRule, IEnumerable<long> saleZoneIds)
        {
            if (salePricingRule.Criteria.SaleZoneGroupSettings != null)
            {
                IRuleSaleZoneCriteria ruleCode = salePricingRule as IRuleSaleZoneCriteria;
                if (ruleCode.SaleZoneIds != null && ruleCode.SaleZoneIds.Intersect(saleZoneIds).Count() > 0)
                    return true;
            }

            return false;
        }
        private bool CheckIfCustomerSettingsContains(SalePricingRule salePricingRule, IEnumerable<int> customerIds)
        {
            if (salePricingRule.Criteria.CustomerGroupSettings != null)
            {
                IRuleCustomerCriteria ruleCode = salePricingRule as IRuleCustomerCriteria;
                if (ruleCode.CustomerIds != null && ruleCode.CustomerIds.Intersect(customerIds).Count() > 0)
                    return true;
            }

            return false;
        }

        protected override SalePricingRuleDetail MapToDetails(SalePricingRule rule)
        {
            return new SalePricingRuleDetail
            {
                Entity = rule,
                RuleTypeName=rule.Settings.RuleType.ToString(),

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

        protected override PricingRuleRateTypeTarget CreateRateTypeTarget(SalePricingRulesInput input)
        {
            return new SalePricingRuleRateTypeTarget
            {
                CustomerId = input.CustomerId,
                SaleZoneId = input.SaleZoneId
            };
        }
    }
}
