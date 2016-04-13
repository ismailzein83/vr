﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Business
{
    public class RouteOptionRuleManager : Vanrise.Rules.RuleManager<RouteOptionRule, RouteOptionRuleDetail>
    {
        public RouteOptionRule GetMatchRule(RouteOptionRuleTarget target)
        {
            var ruleTrees = GetRuleTreesByPriority();
            if (ruleTrees != null)
            {
                foreach (var ruleTree in ruleTrees)
                {
                    var matchRule = ruleTree.GetMatchRule(target) as RouteOptionRule;
                    if (matchRule != null)
                        return matchRule;
                }
            }
            return null;
        }

        Vanrise.Rules.RuleTree[] GetRuleTreesByPriority()
        {
            return GetCachedOrCreate("GetRuleTreesByPriority",
                () =>
                {
                    List<Vanrise.Rules.RuleTree> ruleTrees = new List<Vanrise.Rules.RuleTree>();
                    var structureBehaviors = GetRuleStructureBehaviors();
                    var routeOptionRuleTypes = GetRouteOptionRuleTypesTemplates();

                    int? currentPriority = null;
                    List<Vanrise.Rules.BaseRule> currentRules = null;
                    foreach (var ruleType in routeOptionRuleTypes.OrderBy(itm => GetRuleTypePriority(itm)))
                    {
                        int priority = GetRuleTypePriority(ruleType);
                        if (currentPriority == null || currentPriority.Value != priority)
                        {
                            if (currentRules != null && currentRules.Count > 0)
                                ruleTrees.Add(new Vanrise.Rules.RuleTree(currentRules, structureBehaviors));
                            currentPriority = priority;
                            currentRules = new List<Vanrise.Rules.BaseRule>();
                        }
                        var ruleTypeRules = GetFilteredRules(itm => itm.Settings.ConfigId == ruleType.TemplateConfigID);
                        if (ruleTypeRules != null)
                            currentRules.AddRange(ruleTypeRules);
                    }
                    if (currentRules != null && currentRules.Count > 0)
                        ruleTrees.Add(new Vanrise.Rules.RuleTree(currentRules, structureBehaviors));
                    return ruleTrees.ToArray();
                });
        }

        int GetRuleTypePriority(TemplateConfig ruleTypeConfig)
        {
            return ruleTypeConfig.Settings != null ? (ruleTypeConfig.Settings as RouteOptionRuleTypeSettings).Priority : int.MaxValue;
        }

        public List<Vanrise.Entities.TemplateConfig> GetRouteOptionRuleTypesTemplates()
        {
            TemplateConfigManager manager = new TemplateConfigManager();
            return manager.GetTemplateConfigurations(Constants.RouteOptionRuleConfigType);
        }

        IEnumerable<Vanrise.Rules.BaseRuleStructureBehavior> GetRuleStructureBehaviors()
        {
            List<Vanrise.Rules.BaseRuleStructureBehavior> ruleStructureBehaviors = new List<Vanrise.Rules.BaseRuleStructureBehavior>();
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorBySupplierZone());
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorBySupplier());
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorByCode());
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorBySaleZone());
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorByCustomer());
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorByRoutingProduct());
            return ruleStructureBehaviors;
        }

        public Vanrise.Entities.IDataRetrievalResult<RouteOptionRuleDetail> GetFilteredRouteOptionRules(Vanrise.Entities.DataRetrievalInput<RouteOptionRuleQuery> input)
        {
            var routeOptionRules = base.GetAllRules();
            Func<RouteOptionRule, bool> filterExpression = (routeOptionRule) =>
                (input.Query.RoutingProductId == null || routeOptionRule.Criteria.RoutingProductId == input.Query.RoutingProductId)
                 && (input.Query.Code == null || this.CheckIfCodeCriteriaSettingsContains(routeOptionRule, input.Query.Code))
                 && (input.Query.CustomerIds == null || this.CheckIfCustomerSettingsContains(routeOptionRule, input.Query.CustomerIds))
                 && (input.Query.SaleZoneIds == null || this.CheckIfSaleZoneSettingsContains(routeOptionRule, input.Query.SaleZoneIds));

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, routeOptionRules.ToBigResult(input, filterExpression, MapToDetails));
        }

        protected override RouteOptionRuleDetail MapToDetails(RouteOptionRule rule)
        {
            return new RouteOptionRuleDetail
            {
                Entity = rule

            };
        }

        private bool CheckIfCodeCriteriaSettingsContains(RouteOptionRule routeOptionRule, string code)
        {
            if (routeOptionRule.Criteria.CodeCriteriaGroupSettings != null)
            {
                IRuleCodeCriteria ruleCode = routeOptionRule as IRuleCodeCriteria;
                if (ruleCode.CodeCriterias != null && ruleCode.CodeCriterias.Any(x => x.Code == code))
                    return true;
            }

            return false;
        }

        private bool CheckIfCustomerSettingsContains(RouteOptionRule routeOptionRule, IEnumerable<int> customerIds)
        {
            if (routeOptionRule.Criteria.CustomerGroupSettings != null)
            {
                IRuleCustomerCriteria ruleCode = routeOptionRule as IRuleCustomerCriteria;
                if (ruleCode.CustomerIds != null && ruleCode.CustomerIds.Intersect(customerIds).Count() > 0)
                    return true;
            }

            return false;
        }

        private bool CheckIfSaleZoneSettingsContains(RouteOptionRule routeOptionRule, IEnumerable<long> saleZoneIds)
        {
            if (routeOptionRule.Criteria.SaleZoneGroupSettings != null)
            {
                IRuleSaleZoneCriteria ruleCode = routeOptionRule as IRuleSaleZoneCriteria;
                if (ruleCode.SaleZoneIds != null && ruleCode.SaleZoneIds.Intersect(saleZoneIds).Count() > 0)
                    return true;
            }

            return false;
        }
    }
}
