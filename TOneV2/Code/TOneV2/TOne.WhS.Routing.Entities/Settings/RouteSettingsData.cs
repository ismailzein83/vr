using System;
using System.Collections.Generic;
using System.Linq;
using Vanrise.Entities;

namespace TOne.WhS.Routing.Entities
{
    public class RouteSettingsData : SettingData
    {
        static readonly List<RouteRuleCriteriaPriority> defaultRouteRuleCriteriasPriority = new List<RouteRuleCriteriaPriority>()
        {
            new RouteRuleCriteriaPriority() { Id = BaseRuleCriteriaPriority.s_CodeRuleBehavior, Name = "Code" },
            //new RouteRuleCriteriaPriority() { Id = BaseRuleCriteriaPriority.s_DealRuleBehavior, Name = "Deal" },
            new RouteRuleCriteriaPriority() { Id = BaseRuleCriteriaPriority.s_ZoneRuleBehavior, Name = "Zone" },
            new RouteRuleCriteriaPriority() { Id = BaseRuleCriteriaPriority.s_CountryRuleBehavior, Name = "Country" },
            new RouteRuleCriteriaPriority() { Id = BaseRuleCriteriaPriority.s_CustomerRuleBehavior, Name = "Customer" },
            new RouteRuleCriteriaPriority() { Id = BaseRuleCriteriaPriority.s_RoutingProductRuleBehavior, Name = "Routing Product" }
        };

        static readonly List<RouteOptionRuleCriteriaPriority> defaultRouteOptionRuleCriteriasPriority = new List<RouteOptionRuleCriteriaPriority>()
        {
            new RouteOptionRuleCriteriaPriority(){ Id = BaseRuleCriteriaPriority.s_SupplierZoneRuleBehavior, Name = "SupplierZone" },
            new RouteOptionRuleCriteriaPriority(){ Id = BaseRuleCriteriaPriority.s_SupplierRuleBehavior, Name = "Supplier" },
            new RouteOptionRuleCriteriaPriority(){ Id = BaseRuleCriteriaPriority.s_CodeRuleBehavior, Name = "Code" },
            new RouteOptionRuleCriteriaPriority(){ Id = BaseRuleCriteriaPriority.s_ZoneRuleBehavior, Name = "Zone" },
            new RouteOptionRuleCriteriaPriority(){ Id = BaseRuleCriteriaPriority.s_CountryRuleBehavior, Name = "Country" },
            new RouteOptionRuleCriteriaPriority(){ Id = BaseRuleCriteriaPriority.s_CustomerRuleBehavior, Name = "Customer" }
        };

        public RouteDatabasesToKeep RouteDatabasesToKeep { get; set; }

        public SubProcessSettings SubProcessSettings { get; set; }

        public RouteBuildConfiguration RouteBuildConfiguration { get; set; }

        public RouteRuleConfiguration RouteRuleConfiguration { get; set; }

        public RouteOptionRuleConfiguration RouteOptionRuleConfiguration { get; set; }

        public QualityConfiguration QualityConfiguration { get; set; }

        public override bool IsValid(ISettingDataValidationContext context)
        {
            int nbOfDefaults = 0;
            foreach (var routeRuleQualityConfiguration in this.QualityConfiguration.RouteRuleQualityConfigurationList)
            {
                if (routeRuleQualityConfiguration.IsDefault)
                {
                    nbOfDefaults++;
                    if (nbOfDefaults > 1)
                    {
                        context.ErrorMessage = "Only one default route rule quality configuration is permitted.";
                        return false;
                    }
                    if (!routeRuleQualityConfiguration.IsActive)
                    {
                        context.ErrorMessage = "Default route rule quality configuration should be active.";
                        return false;
                    }
                }
            }
            if (nbOfDefaults == 0)
            {
                context.ErrorMessage = "At least one default route rule quality configuration should be added.";
                return false;
            }
            return true;
        }

        public override void PrepareSettingBeforeLoad(ISettingPrepareSettingBeforeLoadContext context)
        {
            if (RouteRuleConfiguration == null)
            {
                RouteRuleConfiguration = new RouteRuleConfiguration
                {
                    FixedOptionLossDefaultValue = false,
                    FixedOptionLossType = FixedOptionLossType.RemoveLoss,
                    RuleCriteriasPriority = defaultRouteRuleCriteriasPriority
                };
            }
            else
            {
                RouteRuleConfiguration.RuleCriteriasPriority = PrepareRouteRulesPriority(RouteRuleConfiguration.RuleCriteriasPriority, defaultRouteRuleCriteriasPriority);
            }

            if (RouteOptionRuleConfiguration == null)
            {
                RouteOptionRuleConfiguration = new RouteOptionRuleConfiguration
                {
                    RuleCriteriasPriority = defaultRouteOptionRuleCriteriasPriority
                };
            }
            else
            {
                RouteOptionRuleConfiguration.RuleCriteriasPriority = PrepareRouteRulesPriority(RouteOptionRuleConfiguration.RuleCriteriasPriority, defaultRouteOptionRuleCriteriasPriority);
            }
        }

        private List<T> PrepareRouteRulesPriority<T>(List<T> ruleCriteriasPriority, List<T> defaultRuleCriteriasPriority) where T : BaseRuleCriteriaPriority
        {
            if (ruleCriteriasPriority == null || ruleCriteriasPriority.Count == 0 || ruleCriteriasPriority.Count > defaultRuleCriteriasPriority.Count)
                return defaultRuleCriteriasPriority;

            if (ruleCriteriasPriority.Count == defaultRuleCriteriasPriority.Count)
                return ruleCriteriasPriority;

            MergeDefaultWithRuleCriteriasPriority(ruleCriteriasPriority, defaultRuleCriteriasPriority);

            return ruleCriteriasPriority;
        }

        private void MergeDefaultWithRuleCriteriasPriority<T>(List<T> ruleCriteriasPriority, List<T> defaultRuleCriteriasPriority) where T : BaseRuleCriteriaPriority
        {
            List<string> defaultRuleCriteriasNames = defaultRuleCriteriasPriority.Select(item => item.Name).ToList();
            List<string> ruleCriteriasNames = ruleCriteriasPriority.Select(item => item.Name).ToList();
            IEnumerable<string> exceptRuleCriteriasNames = defaultRuleCriteriasNames.Except(ruleCriteriasNames);

            foreach (var exceptRuleCriteriaName in exceptRuleCriteriasNames)
            {
                int currentExceptRuleCriteriaIndex = defaultRuleCriteriasNames.IndexOf(exceptRuleCriteriaName);
                if (currentExceptRuleCriteriaIndex == 0)
                {
                    ruleCriteriasPriority.Insert(0, defaultRuleCriteriasPriority.First());
                    continue;
                }

                if (currentExceptRuleCriteriaIndex == defaultRuleCriteriasNames.Count - 1)
                {
                    ruleCriteriasPriority.Add(defaultRuleCriteriasPriority.Last());
                    continue;
                }

                int lastPreviousRuleCriteriaIndex = 0;
                List<string> previousRuleCriterias = defaultRuleCriteriasNames.GetRange(0, currentExceptRuleCriteriaIndex);

                foreach (var previousRuleCriteria in previousRuleCriterias)
                {
                    int indexOfCurrentPreviousRule = ruleCriteriasNames.IndexOf(previousRuleCriteria);
                    lastPreviousRuleCriteriaIndex = Math.Max(lastPreviousRuleCriteriaIndex, indexOfCurrentPreviousRule);
                }

                ruleCriteriasPriority.Insert(lastPreviousRuleCriteriaIndex + 1, defaultRuleCriteriasPriority[currentExceptRuleCriteriaIndex]);
            }
        }
    }
}