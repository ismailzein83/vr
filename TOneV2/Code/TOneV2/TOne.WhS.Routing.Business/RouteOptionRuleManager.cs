using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Rules;

namespace TOne.WhS.Routing.Business
{
    public class RouteOptionRuleManager : Vanrise.Rules.RuleManager<RouteOptionRule, RouteOptionRuleDetail>
    {
        #region Variables/Ctor

        static RouteOptionRuleManager()
        {
            RouteOptionRuleManager instance = new RouteOptionRuleManager();
            instance.AddRuleCachingExpirationChecker(new RouteOptionRuleCachingExpirationChecker());
        }

        #endregion

        #region Public Methods
        public override bool ValidateBeforeAdd(RouteOptionRule rule)
        {
            Dictionary<int, RouteOptionRule> cachedRules = base.GetAllRules();
            Func<RouteOptionRule, bool> filterExpression = (RouteOptionRule) => string.Compare(RouteOptionRule.Name, rule.Name, true) == 0;
            IEnumerable<RouteOptionRule> result = cachedRules.FindAllRecords(filterExpression);
            return result == null || result.Count() == 0 ? true : false;
        }

        public override bool ValidateBeforeUpdate(RouteOptionRule rule)
        {
            Dictionary<int, RouteOptionRule> cachedRules = base.GetAllRules();
            Func<RouteOptionRule, bool> filterExpression = (RouteOptionRule) => string.Compare(RouteOptionRule.Name, rule.Name, true) == 0 && RouteOptionRule.RuleId != rule.RuleId;
            IEnumerable<RouteOptionRule> result = cachedRules.FindAllRecords(filterExpression);
            return result == null || result.Count() == 0 ? true : false;
        }

        public Vanrise.Entities.IDataRetrievalResult<RouteOptionRuleDetail> GetFilteredRouteOptionRules(Vanrise.Entities.DataRetrievalInput<RouteOptionRuleQuery> input)
        {
            var routeOptionRules = base.GetAllRules();
            string ruleNameLower = !string.IsNullOrEmpty(input.Query.Name) ? input.Query.Name.ToLower() : null;
            Func<RouteOptionRule, bool> filterExpression = (routeOptionRule) =>
            {
                if (!input.Query.RoutingProductId.HasValue && routeOptionRule.Criteria.RoutingProductId.HasValue)
                    return false;

                if (input.Query.RoutingProductId.HasValue && (!routeOptionRule.Criteria.RoutingProductId.HasValue || routeOptionRule.Criteria.RoutingProductId.Value != input.Query.RoutingProductId.Value))
                    return false;

                if (!string.IsNullOrEmpty(ruleNameLower) && (string.IsNullOrEmpty(routeOptionRule.Name) || !routeOptionRule.Name.ToLower().Contains(ruleNameLower)))
                    return false;

                if (!string.IsNullOrEmpty(input.Query.Code) && !CheckIfCodeCriteriaSettingsContains(routeOptionRule, input.Query.Code))
                    return false;

                if (input.Query.CustomerIds != null && !CheckIfCustomerSettingsContains(routeOptionRule, input.Query.CustomerIds))
                    return false;

                if (input.Query.SaleZoneIds != null && !CheckIfSaleZoneSettingsContains(routeOptionRule, input.Query.SaleZoneIds))
                    return false;

                if (input.Query.RouteOptionRuleSettingsConfigIds != null && !CheckIfSameRouteOptionRuleSettingsConfigId(routeOptionRule, input.Query.RouteOptionRuleSettingsConfigIds))
                    return false;

                if (input.Query.EffectiveOn.HasValue && (routeOptionRule.BeginEffectiveTime > input.Query.EffectiveOn || (routeOptionRule.EndEffectiveTime.HasValue && routeOptionRule.EndEffectiveTime <= input.Query.EffectiveOn)))
                    return false;

                return true;
            };

            return Vanrise.Common.DataRetrievalManager.Instance.ProcessResult(input, routeOptionRules.ToBigResult(input, filterExpression, MapToDetails));
        }

        public RouteOptionRuleEditorRuntime GetRuleEditorRuntime(int ruleId)
        {
            RouteOptionRuleEditorRuntime routeOptionRuleEditorRuntime = new RouteOptionRuleEditorRuntime();
            routeOptionRuleEditorRuntime.Entity = base.GetRule(ruleId);

            if (routeOptionRuleEditorRuntime.Entity == null)
                throw new NullReferenceException(string.Format("routeOptionRuleEditorRuntime.Entity for Rule ID: {0} is null", ruleId));

            if (routeOptionRuleEditorRuntime.Entity.Settings == null)
                throw new NullReferenceException(string.Format("routeOptionRuleEditorRuntime.Entity.Settings for Rule ID: {0} is null", ruleId));

            routeOptionRuleEditorRuntime.SettingsEditorRuntime = routeOptionRuleEditorRuntime.Entity.Settings.GetEditorRuntime();

            return routeOptionRuleEditorRuntime;
        }

        public Vanrise.Rules.RuleTree[] GetRuleTreesByPriorityForCustomerRoutes()
        {
            return GetCachedOrCreate("GetRuleTreesByPriorityForCustomerRoutes",
                () =>
                {
                    return BuildRuleTree((allRouteOptionRuleConfig) =>
                    {
                        List<RouteOptionRuleConfig> results = new List<RouteOptionRuleConfig>();
                        RouteOptionRuleTypeConfiguration routeOptionRuleTypeConfiguration;
                        Dictionary<Guid, RouteOptionRuleTypeConfiguration> routeOptionRuleTypeConfigurationDic = new ConfigManager().GetRouteOptionRuleTypeConfigurationForCustomerRoutes();

                        foreach (var itm in allRouteOptionRuleConfig)
                        {
                            routeOptionRuleTypeConfiguration = routeOptionRuleTypeConfigurationDic.GetRecord(itm.ExtensionConfigurationId);
                            if (!itm.CanExcludeFromRouteBuildProcess || (routeOptionRuleTypeConfiguration != null && !routeOptionRuleTypeConfiguration.IsExcluded))
                                results.Add(itm);
                        }
                        return results;
                    });
                });
        }

        public Vanrise.Rules.RuleTree[] GetRuleTreesByPriorityForProductRoutes()
        {
            return GetCachedOrCreate("GetRuleTreesByPriorityForProductRoutes",
                () =>
                {
                    return BuildRuleTree((allRouteOptionRuleConfig) =>
                    {
                        List<RouteOptionRuleConfig> results = new List<RouteOptionRuleConfig>();
                        RouteOptionRuleTypeConfiguration routeOptionRuleTypeConfiguration;
                        Dictionary<Guid, RouteOptionRuleTypeConfiguration> routeOptionRuleTypeConfigurationDic = new ConfigManager().GetRouteOptionRuleTypeConfigurationForProductRoutes();

                        foreach (var itm in allRouteOptionRuleConfig)
                        {
                            routeOptionRuleTypeConfiguration = routeOptionRuleTypeConfigurationDic.GetRecord(itm.ExtensionConfigurationId);
                            if (!itm.CanExcludeFromProductCostProcess || (routeOptionRuleTypeConfiguration != null && !routeOptionRuleTypeConfiguration.IsExcluded))
                                results.Add(itm);
                        }
                        return results;
                    });
                });
        }

        private Vanrise.Rules.RuleTree[] BuildRuleTree(Func<IEnumerable<RouteOptionRuleConfig>, IEnumerable<RouteOptionRuleConfig>> GetIncludedRouteOptionRuleTypes)
        {
            List<Vanrise.Rules.RuleTree> ruleTrees = new List<Vanrise.Rules.RuleTree>();
            var structureBehaviors = GetRuleStructureBehaviors();
            var routeOptionRuleTypes = GetRouteOptionRuleTypesTemplates();
            IEnumerable<RouteOptionRuleConfig> includedRouteOptionRuleTypes = GetIncludedRouteOptionRuleTypes(routeOptionRuleTypes);

            int? currentPriority = null;
            List<Vanrise.Rules.IVRRule> currentRules = null;
            foreach (var ruleType in includedRouteOptionRuleTypes.OrderBy(itm => GetRuleTypePriority(itm)))
            {
                int priority = GetRuleTypePriority(ruleType);
                if (currentPriority == null || currentPriority.Value != priority)
                {
                    if (currentRules != null && currentRules.Count > 0)
                        ruleTrees.Add(new Vanrise.Rules.RuleTree(currentRules, structureBehaviors));
                    currentPriority = priority;
                    currentRules = new List<Vanrise.Rules.IVRRule>();
                }
                var ruleTypeRules = GetFilteredRules(itm => itm.Settings.ConfigId == ruleType.ExtensionConfigurationId);
                if (ruleTypeRules != null)
                    currentRules.AddRange(ruleTypeRules);
            }
            if (currentRules != null && currentRules.Count > 0)
                ruleTrees.Add(new Vanrise.Rules.RuleTree(currentRules, structureBehaviors));
            return ruleTrees.ToArray();
        }

        public IEnumerable<RouteOptionRuleConfig> GetRouteOptionRuleTypesTemplates()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurations<RouteOptionRuleConfig>(RouteOptionRuleConfig.EXTENSION_TYPE);
        }

        public override RouteOptionRuleDetail MapToDetails(RouteOptionRule rule)
        {
            string _cssClass = null;
            string _routeoptionRuleSettingsTypeName = null;
            Dictionary<Guid, RouteOptionRuleConfig> routeOptionRuleConfigDic = GetRouteOptionRuleTypesTemplatesDict();


            if (rule != null && rule.Settings != null && rule.Settings.ConfigId != null)
            {
                RouteOptionRuleConfig routeRuleSettingsConfig = routeOptionRuleConfigDic.GetRecord(rule.Settings.ConfigId);
                _cssClass = routeRuleSettingsConfig.CssClass;
                _routeoptionRuleSettingsTypeName = routeRuleSettingsConfig.Title;
            }

            return new RouteOptionRuleDetail
            {
                Entity = rule,
                CssClass = _cssClass,
                RouteOptionRuleSettingsTypeName = _routeoptionRuleSettingsTypeName
            };
        }

        public Dictionary<Guid, RouteOptionRuleConfig> GetRouteOptionRuleTypesTemplatesDict()
        {
            ExtensionConfigurationManager manager = new ExtensionConfigurationManager();
            return manager.GetExtensionConfigurationsByType<RouteOptionRuleConfig>(RouteOptionRuleConfig.EXTENSION_TYPE);
        }

        public IEnumerable<ProcessRouteOptionRuleConfig> GetRouteOptionRuleSettingsTemplatesByProcessType(RoutingProcessType routingProcessType)
        {
            List<ProcessRouteOptionRuleConfig> results = new List<ProcessRouteOptionRuleConfig>();

            IEnumerable<RouteOptionRuleConfig> allRouteOptionRuleConfig = GetRouteOptionRuleTypesTemplates();

            if (allRouteOptionRuleConfig == null)
                return null;

            switch (routingProcessType)
            {
                case RoutingProcessType.CustomerRoute:
                    foreach (var itm in allRouteOptionRuleConfig)
                    {
                        results.Add(new ProcessRouteOptionRuleConfig
                        {
                            ExtensionConfigurationId = itm.ExtensionConfigurationId,
                            Title = itm.Title,
                            CanExclude = itm.CanExcludeFromRouteBuildProcess
                        });
                    }
                    break;

                case RoutingProcessType.RoutingProductRoute:
                    foreach (var itm in allRouteOptionRuleConfig)
                    {
                        results.Add(new ProcessRouteOptionRuleConfig
                        {
                            ExtensionConfigurationId = itm.ExtensionConfigurationId,
                            Title = itm.Title,
                            CanExclude = itm.CanExcludeFromProductCostProcess
                        });
                    }
                    break;

                default: throw new Exception(string.Format("Unsupported RoutingProcessType: {0}", routingProcessType));
            }
            return results;
        }

        #endregion

        #region Private Methods

        private IEnumerable<Vanrise.Rules.BaseRuleStructureBehavior> GetRuleStructureBehaviors()
        {
            List<Vanrise.Rules.BaseRuleStructureBehavior> ruleStructureBehaviors = new List<Vanrise.Rules.BaseRuleStructureBehavior>();
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorBySupplierZone());
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorBySupplier());
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorByCode());
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorBySaleZone());
            ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorByCustomer());
            //ruleStructureBehaviors.Add(new TOne.WhS.BusinessEntity.Business.Rules.StructureRuleBehaviors.RuleBehaviorByRoutingProduct());
            return ruleStructureBehaviors;
        }

        private int GetRuleTypePriority(RouteOptionRuleConfig ruleTypeConfig)
        {
            return ruleTypeConfig.Priority.HasValue ? ruleTypeConfig.Priority.Value : int.MaxValue;
        }

        private bool CheckIfCodeCriteriaSettingsContains(RouteOptionRule routeOptionRule, string code)
        {
            if (routeOptionRule.Criteria.CodeCriteriaGroupSettings != null)
            {
                IRuleCodeCriteria ruleCode = routeOptionRule as IRuleCodeCriteria;
                if (ruleCode.CodeCriterias != null && ruleCode.CodeCriterias.Any(x => x.Code.StartsWith(code)))
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
        private bool CheckIfSameRouteOptionRuleSettingsConfigId(RouteOptionRule routeOptionRule, List<Guid> RouteOptionRuleSettingsConfigIds)
        {
            if (RouteOptionRuleSettingsConfigIds.Contains(routeOptionRule.Settings.ConfigId))
                return true;

            return false;
        }

        #endregion
    }

    public class RouteOptionRuleCachingExpirationChecker : RuleCachingExpirationChecker
    {
        DateTime? _dataRecordTypeCacheLastCheck;

        public override bool IsRuleDependenciesCacheExpired()
        {
            return Vanrise.Caching.CacheManagerFactory.GetCacheManager<SettingManager.CacheManager>().IsCacheExpired(ref _dataRecordTypeCacheLastCheck);
        }
    }
}