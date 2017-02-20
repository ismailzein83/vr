using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.MainExtensions.CodeCriteriaGroups;
using TOne.WhS.BusinessEntity.MainExtensions.CustomerGroups;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Business
{
    public class RouteRuleManager : Vanrise.Rules.RuleManager<RouteRule, RouteRuleDetail>
    {
        #region Public Methods

        public IEnumerable<RouteRule> GetEffectiveLinkedRouteRules(int customerId, string code, DateTime effectiveDate)
        {
            string itemKey = string.Format("{0}@{1}", customerId, code);
            var linkedRules = GetCachedLinkedRouteRules();
            if (linkedRules == null)
                return null;

            List<RouteRule> linkedRouteRules = linkedRules.GetRecord(itemKey);
            if (linkedRouteRules == null)
                return null;
            return linkedRouteRules.FindAllRecords(itm => itm.IsEffectiveOrFuture(effectiveDate));
        }

        public RouteRule BuildLinkedRouteRule(int ruleId, int? customerId, string code, List<RouteOption> routeOptions)
        {
            RouteRule relatedRouteRule = base.GetRule(ruleId);
            if (relatedRouteRule == null)
                throw new NullReferenceException(string.Format("relatedRouteRule. RuleId: {0}", ruleId));

            if (relatedRouteRule.Settings == null)
                throw new NullReferenceException(string.Format("relatedRouteRule.Settings. RuleId: {0}", ruleId));

            LinkedRouteRuleContext context = new LinkedRouteRuleContext() { RouteOptions = routeOptions };

            RouteRule linkedRouteRule = new RouteRule()
            {
                BeginEffectiveTime = DateTime.Now,
                Settings = relatedRouteRule.Settings.BuildLinkedRouteRuleSettings(context),
                Criteria = new RouteRuleCriteria()
            };

            if (customerId.HasValue && customerId.Value > 0)
                linkedRouteRule.Criteria.CustomerGroupSettings = new SelectiveCustomerGroup() { CustomerIds = new List<int>() { customerId.Value } };


            if (!string.IsNullOrEmpty(code))
            {
                CodeCriteria codeCriteria = new BusinessEntity.Entities.CodeCriteria() { Code = code };
                linkedRouteRule.Criteria.CodeCriteriaGroupSettings = new SelectiveCodeCriteriaGroup() { Codes = new List<CodeCriteria>() { codeCriteria } };
            }

            return linkedRouteRule;
        }

        //public override bool ValidateBeforeAdd(RouteRule rule)
        //{
        //    Dictionary<int, RouteRule> cachedRules = base.GetAllRules();
        //    Func<RouteRule, bool> filterExpression = (routeRule) => string.Compare(routeRule.Name, rule.Name, true) == 0;
        //    IEnumerable<RouteRule> result = cachedRules.FindAllRecords(filterExpression);
        //    return result == null || result.Count() == 0 ? true : false;
        //}

        //public override bool ValidateBeforeUpdate(RouteRule rule)
        //{
        //    Dictionary<int, RouteRule> cachedRules = base.GetAllRules();
        //    Func<RouteRule, bool> filterExpression = (routeRule) => string.Compare(routeRule.Name, rule.Name, true) == 0 && routeRule.RuleId != rule.RuleId;
        //    IEnumerable<RouteRule> result = cachedRules.FindAllRecords(filterExpression);
        //    return result == null || result.Count() == 0 ? true : false;
        //}

        public Vanrise.Entities.IDataRetrievalResult<RouteRuleDetail> GetFilteredRouteRules(Vanrise.Entities.DataRetrievalInput<RouteRuleQuery> input)
        {
            var routeRules = base.GetAllRules();
            string ruleNameLower = !string.IsNullOrEmpty(input.Query.Name) ? input.Query.Name.ToLower() : null;
            Func<RouteRule, bool> filterExpression = (routeRule) =>
                {
                    if (!input.Query.RoutingProductId.HasValue && routeRule.Criteria.RoutingProductId.HasValue)
                        return false;

                    if (input.Query.RoutingProductId.HasValue && (!routeRule.Criteria.RoutingProductId.HasValue || routeRule.Criteria.RoutingProductId.Value != input.Query.RoutingProductId.Value))
                        return false;

                    if (!string.IsNullOrEmpty(ruleNameLower) && (string.IsNullOrEmpty(routeRule.Name) || !routeRule.Name.ToLower().Contains(ruleNameLower)))
                        return false;

                    if (!string.IsNullOrEmpty(input.Query.Code) && !CheckIfCodeCriteriaSettingsContains(routeRule, input.Query.Code))
                        return false;

                    if (input.Query.CustomerIds != null && !CheckIfCustomerSettingsContains(routeRule, input.Query.CustomerIds))
                        return false;

                    if (input.Query.SaleZoneIds != null && !CheckIfSaleZoneSettingsContains(routeRule, input.Query.SaleZoneIds))
                        return false;

                    if (input.Query.RouteRuleSettingsConfigIds != null && !CheckIfSameRouteRuleSettingsConfigId(routeRule, input.Query.RouteRuleSettingsConfigIds))
                        return false;

                    if (input.Query.EffectiveOn.HasValue && (routeRule.BeginEffectiveTime > input.Query.EffectiveOn || (routeRule.EndEffectiveTime.HasValue && routeRule.EndEffectiveTime <= input.Query.EffectiveOn)))
                        return false;

                    if (input.Query.LinkedRouteRuleIds != null && !input.Query.LinkedRouteRuleIds.Contains(routeRule.RuleId))
                        return false;

                    return true;
                };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, routeRules.ToBigResult(input, filterExpression, MapToDetails));
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

        public Vanrise.Rules.RuleTree[] GetRuleTreesByPriority(int? routingProductId)
        {
            return GetCachedOrCreate(string.Format("GetRuleTreesByPriority_{0}", routingProductId.HasValue ? routingProductId.Value : 0),
                () =>
                {
                    List<Vanrise.Rules.RuleTree> ruleTrees = new List<Vanrise.Rules.RuleTree>();
                    var structureBehaviors = GetRuleStructureBehaviors();
                    var routeRuleTypes = GetRouteRuleTypesTemplates();

                    int? currentPriority = null;
                    List<Vanrise.Rules.IVRRule> currentRules = null;
                    foreach (var ruleType in routeRuleTypes.OrderBy(itm => GetRuleTypePriority(itm)))
                    {
                        int priority = GetRuleTypePriority(ruleType);
                        if (currentPriority == null || currentPriority.Value != priority)
                        {
                            if (currentRules != null && currentRules.Count > 0)
                                ruleTrees.Add(new Vanrise.Rules.RuleTree(currentRules, structureBehaviors));
                            currentPriority = priority;
                            currentRules = new List<Vanrise.Rules.IVRRule>();
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

        public IEnumerable<RouteRuleSettingsConfig> GetRouteRuleTypesTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<RouteRuleSettingsConfig>(RouteRuleSettingsConfig.EXTENSION_TYPE);
        }

        public override RouteRuleDetail MapToDetails(RouteRule rule)
        {
            string _cssClass = null;
            string _routeRuleSettingsTypeName = null;
            Dictionary<Guid, RouteRuleSettingsConfig> routeRuleSettingsConfigDic = GetRouteRuleTypesTemplatesDict();

            if (rule != null && rule.Settings != null && rule.Settings.ConfigId != null)
            {
                RouteRuleSettingsConfig routeRuleSettingsConfig = routeRuleSettingsConfigDic.GetRecord(rule.Settings.ConfigId);
                _cssClass = routeRuleSettingsConfig.CssClass;
                _routeRuleSettingsTypeName = routeRuleSettingsConfig.Title;
            }

            return new RouteRuleDetail()
            {
                Entity = rule,
                CssClass = _cssClass,
                RouteRuleSettingsTypeName = _routeRuleSettingsTypeName
            };
        }

        public Dictionary<Guid, RouteRuleSettingsConfig> GetRouteRuleTypesTemplatesDict()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurationsByType<RouteRuleSettingsConfig>(RouteRuleSettingsConfig.EXTENSION_TYPE);
        }

        #endregion


        #region Private Methods

        private IEnumerable<Vanrise.Rules.BaseRuleStructureBehavior> GetRuleStructureBehaviors()
        {
            List<Vanrise.Rules.BaseRuleStructureBehavior> ruleStructureBehaviors = new List<Vanrise.Rules.BaseRuleStructureBehavior>();
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorByCode());
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorBySaleZone());
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorByCustomer());
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorByRoutingProduct());
            return ruleStructureBehaviors;
        }

        private int GetRuleTypePriority(RouteRuleSettingsConfig ruleTypeConfig)
        {
            return ruleTypeConfig.Priority.HasValue ? ruleTypeConfig.Priority.Value : int.MaxValue;
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
        private bool CheckIfSameRouteRuleSettingsConfigId(RouteRule routeRule, List<Guid> RouteRuleSettingsConfigIds)
        {
            if (RouteRuleSettingsConfigIds.Contains(routeRule.Settings.ConfigId))
                return true;

            return false;
        }

        private Dictionary<string, List<RouteRule>> GetCachedLinkedRouteRules()
        {
            return GetCachedOrCreate(string.Format("GetCachedLinkedRouteRules"),
                () =>
                {
                    Dictionary<string, List<RouteRule>> linkedRouteRules = new Dictionary<string, List<RouteRule>>();
                    Dictionary<int, RouteRule> routeRules = GetAllRules();

                    if (routeRules != null)
                    {
                        foreach (var routeRule in routeRules)
                        {
                            if (routeRule.Value.Criteria != null)
                            {
                                RouteRuleCriteria criteria = routeRule.Value.Criteria;

                                string code = CheckAndReturnValidCode(criteria);
                                if (string.IsNullOrEmpty(code))
                                    continue;

                                int? customerId = CheckAndReturnValidCustomer(criteria);
                                if (!customerId.HasValue)
                                    continue;

                                string itemKey = string.Format("{0}@{1}", customerId.Value, code);
                                List<RouteRule> relatedRouteRules = linkedRouteRules.GetOrCreateItem(itemKey);
                                relatedRouteRules.Add(routeRule.Value);
                            }
                        }
                    }

                    return linkedRouteRules;
                });
        }

        public int? CheckAndReturnValidCustomer(RouteRuleCriteria criteria)
        {
            if (criteria.CustomerGroupSettings == null)
                return null;

            SelectiveCustomerGroup selectiveCustomerGroup = criteria.CustomerGroupSettings as SelectiveCustomerGroup;
            if (selectiveCustomerGroup == null)
                return null;

            if (selectiveCustomerGroup.CustomerIds == null || selectiveCustomerGroup.CustomerIds.Count != 1)
                return null;

            return selectiveCustomerGroup.CustomerIds.First();
        }

        public string CheckAndReturnValidCode(RouteRuleCriteria criteria)
        {
            if (criteria.CodeCriteriaGroupSettings == null)
                return null;

            SelectiveCodeCriteriaGroup selectiveCodeCriteriaGroup = criteria.CodeCriteriaGroupSettings as SelectiveCodeCriteriaGroup;
            if (selectiveCodeCriteriaGroup == null)
                return null;

            if (selectiveCodeCriteriaGroup.Codes == null || selectiveCodeCriteriaGroup.Codes.Count != 1)
                return null;
            string code = selectiveCodeCriteriaGroup.Codes.First().Code;

            if (criteria.ExcludedCodes != null && criteria.ExcludedCodes.Contains(code))
                return null;

            return code;
        }
        #endregion
    }
}
