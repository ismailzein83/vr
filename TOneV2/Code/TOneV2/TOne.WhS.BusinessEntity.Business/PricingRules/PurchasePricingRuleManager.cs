﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;
namespace TOne.WhS.BusinessEntity.Business
{
    public class PurchasePricingRuleManager : BasePricingRuleManager<PurchasePricingRule, PurchasePricingRuleDetail, PurchasePricingRulesInput>
    {
 
        #region Public Methods
        public Vanrise.Entities.IDataRetrievalResult<PurchasePricingRuleDetail> GetFilteredPurchasePricingRules(Vanrise.Entities.DataRetrievalInput<PurchasePricingRuleQuery> input)
        {
            Func<PurchasePricingRule, bool> filterExpression = (prod) =>
                (input.Query.Description == null || prod.Description.ToLower().Contains(input.Query.Description.ToLower()))
                &&
                (input.Query.RuleTypes == null || input.Query.RuleTypes.Contains(prod.Settings.RuleType))
                 &&
                (input.Query.SupplierIds == null || this.CheckIfSupplierWithZonesSettingsContains(prod, input.Query.SupplierIds))
                  && (input.Query.EffectiveDate == null || (prod.BeginEffectiveTime <= input.Query.EffectiveDate && (prod.EndEffectiveTime == null || prod.EndEffectiveTime >= input.Query.EffectiveDate)));
            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, base.GetFilteredRules(filterExpression).ToBigResult(input, filterExpression, MapToDetails));
        }
        #endregion

        #region Protected Methods
        protected override IEnumerable<Vanrise.Rules.BaseRuleStructureBehavior> GetBehaviors()
        {
            List<Vanrise.Rules.BaseRuleStructureBehavior> behaviors = new List<Vanrise.Rules.BaseRuleStructureBehavior>();
            behaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorBySupplier());
            behaviors.Add(new Rules.StructureRuleBehaviors.RuleBehaviorBySupplierZone());

            return behaviors;
        }
        public override PurchasePricingRuleDetail MapToDetails(PurchasePricingRule rule)
        {
            return new PurchasePricingRuleDetail
            {
                Entity = rule,
                RuleTypeName = rule.Settings.RuleType.ToString(),

            };
        }
        protected override PricingRuleTariffTarget CreateTariffTarget(PurchasePricingRulesInput input)
        {
            return new PurchasePricingRuleTariffTarget
            {
                SupplierId = input.SupplierId,
                SupplierZoneId = input.SupplierZoneId,
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
        protected override PricingRuleRateTypeTarget CreateRateTypeTarget(PurchasePricingRulesInput input)
        {
            return new PurchasePricingRuleRateTypeTarget
            {
                SupplierId = input.SupplierId,
                SupplierZoneId = input.SupplierZoneId
            };
        }
        #endregion

        #region Private Methods
        private bool CheckIfSupplierWithZonesSettingsContains(PurchasePricingRule purchasePricingRule, IEnumerable<int> supplierIds)
        {
            if (purchasePricingRule.Criteria.SuppliersWithZonesGroupSettings != null)
            {
                IRuleSupplierCriteria ruleCode = purchasePricingRule as IRuleSupplierCriteria;
                if (ruleCode.SupplierIds != null && ruleCode.SupplierIds.Intersect(supplierIds).Count() > 0)
                    return true;
            }

            return false;
        }
        #endregion
       
    }
}
