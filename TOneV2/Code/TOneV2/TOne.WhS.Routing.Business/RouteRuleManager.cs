using System;
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
    public class RouteRuleManager : Vanrise.Rules.RuleManager<RouteRule, RouteRuleDetail>
    {
        public override bool ValidateBeforeAdd(RouteRule rule)
        {
            Dictionary<int, RouteRule> cachedRules = base.GetAllRules();
            Func<RouteRule, bool> filterExpression = (routeRule) => string.Compare(routeRule.Name, rule.Name, true) == 0;
            IEnumerable<RouteRule> result = cachedRules.FindAllRecords(filterExpression);
            return result == null || result.Count() == 0 ? true : false;
        }

        public override bool ValidateBeforeUpdate(RouteRule rule)
        {
            Dictionary<int, RouteRule> cachedRules = base.GetAllRules();
            Func<RouteRule, bool> filterExpression = (routeRule) => string.Compare(routeRule.Name, rule.Name, true) == 0 && routeRule.RuleId != rule.RuleId;
            IEnumerable<RouteRule> result = cachedRules.FindAllRecords(filterExpression);
            return result == null || result.Count() == 0 ? true : false;
        }
        public RouteRule GetMatchRule(RouteRuleTarget target, int? routingProductId)
        {
            var ruleTrees = GetRuleTreesByPriority(routingProductId);
            if (ruleTrees != null)
            {
                foreach (var ruleTree in ruleTrees)
                {
                    var matchRule = ruleTree.GetMatchRule(target) as RouteRule;
                    if (matchRule != null)
                        return matchRule;
                }
            }
            return null;
        }

        internal Vanrise.Rules.RuleTree[] GetRuleTreesByPriority(int? routingProductId)
        {
            return GetCachedOrCreate(string.Format("GetRuleTreesByPriority_{0}", routingProductId.HasValue ? routingProductId.Value : 0),
                () =>
                {
                    List<Vanrise.Rules.RuleTree> ruleTrees = new List<Vanrise.Rules.RuleTree>();
                    var structureBehaviors = GetRuleStructureBehaviors();
                    var routeRuleTypes = GetRouteRuleTypesTemplates();

                    int? currentPriority = null;
                    List<Vanrise.Rules.BaseRule> currentRules = null;
                    foreach (var ruleType in routeRuleTypes.OrderBy(itm => GetRuleTypePriority(itm)))
                    {
                        int priority = GetRuleTypePriority(ruleType);
                        if (currentPriority == null || currentPriority.Value != priority)
                        {
                            if (currentRules != null && currentRules.Count > 0)
                                ruleTrees.Add(new Vanrise.Rules.RuleTree(currentRules, structureBehaviors));
                            currentPriority = priority;
                            currentRules = new List<Vanrise.Rules.BaseRule>();
                        }
                        var ruleTypeRules = GetFilteredRules(itm => itm.Settings.ConfigId == ruleType.ExtensionConfigurationId
                                                         && (!routingProductId.HasValue || (itm.Criteria.RoutingProductId.HasValue && routingProductId.Value == itm.Criteria.RoutingProductId.Value)));
                        if (ruleTypeRules != null)
                            currentRules.AddRange(ruleTypeRules);
                    }
                    if (currentRules != null && currentRules.Count > 0)
                        ruleTrees.Add(new Vanrise.Rules.RuleTree(currentRules, structureBehaviors));
                    return ruleTrees.ToArray();
                });
        }

        int GetRuleTypePriority(RouteRuleSettingsConfig ruleTypeConfig)
        {
            return ruleTypeConfig.Priority.HasValue ? ruleTypeConfig.Priority.Value : int.MaxValue;
        }

        public IEnumerable<RouteRuleSettingsConfig> GetRouteRuleTypesTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<RouteRuleSettingsConfig>(RouteRuleSettingsConfig.EXTENSION_TYPE);
        }

        public Dictionary<Guid, RouteRuleSettingsConfig> GetRouteRuleTypesTemplatesDict()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurationsByType<RouteRuleSettingsConfig>(RouteRuleSettingsConfig.EXTENSION_TYPE);
        }


        IEnumerable<Vanrise.Rules.BaseRuleStructureBehavior> GetRuleStructureBehaviors()
        {
            List<Vanrise.Rules.BaseRuleStructureBehavior> ruleStructureBehaviors = new List<Vanrise.Rules.BaseRuleStructureBehavior>();
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorByCode());
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorBySaleZone());
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorByCustomer());
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorByRoutingProduct());
            return ruleStructureBehaviors;
        }

        public Vanrise.Entities.IDataRetrievalResult<RouteRuleDetail> GetFilteredRouteRules(Vanrise.Entities.DataRetrievalInput<RouteRuleQuery> input)
        {
            var routeRules = base.GetAllRules();
            Func<RouteRule, bool> filterExpression = (routeRule) =>
                (input.Query.RoutingProductId == null || routeRule.Criteria.RoutingProductId == input.Query.RoutingProductId)
                 && (string.IsNullOrEmpty(input.Query.Name) || (!string.IsNullOrEmpty(routeRule.Name) && routeRule.Name.ToLower().Contains(input.Query.Name.ToLower())))
                 && (string.IsNullOrEmpty(input.Query.Code) || this.CheckIfCodeCriteriaSettingsContains(routeRule, input.Query.Code))
                 && (input.Query.CustomerIds == null || this.CheckIfCustomerSettingsContains(routeRule, input.Query.CustomerIds))
                 && (input.Query.SaleZoneIds == null || this.CheckIfSaleZoneSettingsContains(routeRule, input.Query.SaleZoneIds))
                 && (!input.Query.EffectiveOn.HasValue || routeRule.BeginEffectiveTime <= input.Query.EffectiveOn)
                 && (!input.Query.EffectiveOn.HasValue || !routeRule.EndEffectiveTime.HasValue || routeRule.EndEffectiveTime > input.Query.EffectiveOn);

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, routeRules.ToBigResult(input, filterExpression, MapToDetails));
        }

        public override RouteRuleDetail MapToDetails(RouteRule rule)
        {
            string _cssClass = null;
            Dictionary<Guid, RouteRuleSettingsConfig> routeRuleSettingsConfigDic = GetRouteRuleTypesTemplatesDict();

            if (rule != null && rule.Settings != null && rule.Settings.ConfigId != null)
            {
                _cssClass = routeRuleSettingsConfigDic.GetRecord(rule.Settings.ConfigId).CssClass;
            }

            return new RouteRuleDetail()
            {
                Entity = rule,
                CssClass = _cssClass
            };
        }

        private bool CheckIfCodeCriteriaSettingsContains(RouteRule routeRule, string code)
        {
            if (routeRule.Criteria.CodeCriteriaGroupSettings != null)
            {
                IRuleCodeCriteria ruleCode = routeRule as IRuleCodeCriteria;
                if (ruleCode.CodeCriterias != null && ruleCode.CodeCriterias.Any(x => x.Code.StartsWith(code)))
                    return true;
            }

            return false;
        }
        private bool CheckIfCustomerSettingsContains(RouteRule routeRule, IEnumerable<int> customerIds)
        {
            if (routeRule.Criteria.CustomerGroupSettings != null)
            {
                IRuleCustomerCriteria ruleCode = routeRule as IRuleCustomerCriteria;
                if (ruleCode.CustomerIds != null && ruleCode.CustomerIds.Intersect(customerIds).Count() > 0)
                    return true;
            }

            return false;
        }

        private bool CheckIfSaleZoneSettingsContains(RouteRule routeRule, IEnumerable<long> saleZoneIds)
        {
            if (routeRule.Criteria.SaleZoneGroupSettings != null)
            {
                IRuleSaleZoneCriteria ruleCode = routeRule as IRuleSaleZoneCriteria;
                if (ruleCode.SaleZoneIds != null && ruleCode.SaleZoneIds.Intersect(saleZoneIds).Count() > 0)
                    return true;
            }

            return false;
        }
    }
}
