using System;
using System.Activities;
using System.Collections.Generic;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Rules;
using Vanrise.Rules.Entities;
using Vanrise.Common;

namespace TOne.WhS.Routing.BP.Activities
{
    public sealed class LoadAffectedRules : CodeActivity
    {
        [RequiredArgument]
        public InArgument<DateTime> EffectiveDate { get; set; }

        [RequiredArgument]
        public InArgument<PartialRouteInfo> PartialRouteInfo { get; set; }

        [RequiredArgument]
        public InArgument<int?> RouteRuleId { get; set; }

        [RequiredArgument]
        public OutArgument<AffectedRouteRules> AffectedRouteRules { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime effectiveDate = this.EffectiveDate.Get(context);
            PartialRouteInfo partialRouteInfo = this.PartialRouteInfo.Get(context);

            int? routeRuleId = this.RouteRuleId.Get(context);

            RouteRuleManager routeRuleManager = new RouteRuleManager();
            List<RuleChangedData<RouteRule>> ruleChangedList;

            if (routeRuleId.HasValue)
                ruleChangedList = new List<RuleChangedData<RouteRule>>() { routeRuleManager.GetRuleChanged(routeRuleId.Value) };
            else
                ruleChangedList = routeRuleManager.GetRulesChanged();

            AffectedRouteRules affectedRouteRules = new AffectedRouteRules()
            {
                AddedRouteRules = new List<RouteRule>(),
                UpdatedRouteRules = new List<RouteRule>(),
                OpenedRouteRules = new List<RouteRule>(),
                ClosedRouteRules = new List<RouteRule>()
            };
            HashSet<int> affectedRuleIds = new HashSet<int>();
            if (ruleChangedList != null)
            {
                foreach (var ruleChanged in ruleChangedList)
                {
                    switch (ruleChanged.ActionType)
                    {
                        case ActionType.AddedRule:
                            affectedRuleIds.Add(ruleChanged.RuleId);

                            if (ruleChanged.InitialRule.IsEffective(effectiveDate))
                                affectedRouteRules.AddedRouteRules.Add(ruleChanged.InitialRule);
                            break;

                        case ActionType.UpdatedRule:
                            affectedRouteRules.UpdatedRouteRules.Add(ruleChanged.InitialRule);

                            RouteRuleAdditionalInformation routeRuleAdditionalInformation = ruleChanged.AdditionalInformation.CastWithValidate<RouteRuleAdditionalInformation>("ruleChanged.AdditionalInformation", ruleChanged.RuleId);
                            if (routeRuleAdditionalInformation.CriteriaHasChanged)
                            {
                                affectedRuleIds.Add(ruleChanged.RuleId);
                                RouteRule routeRule = routeRuleManager.GetRouteRule(ruleChanged.RuleId);
                                affectedRouteRules.UpdatedRouteRules.Add(routeRule);
                            }
                            break;

                        default: throw new NotSupportedException(string.Format("ActionType {0} not supported.", ruleChanged.ActionType));
                    }
                }
            }

            if (effectiveDate > partialRouteInfo.LatestRoutingDate)
            {
                Dictionary<int, RouteRule> routeRules = routeRuleManager.GetAllRules();
                if (routeRules != null)
                {
                    foreach (var routeRuleKvp in routeRules)
                    {
                        int ruleId = routeRuleKvp.Key;
                        if (affectedRuleIds.Contains(ruleId))
                            continue;

                        RouteRule routeRule = routeRuleKvp.Value;
                        if (routeRule.IsEffective(effectiveDate) && !routeRule.IsEffective(partialRouteInfo.LatestRoutingDate))
                            affectedRouteRules.OpenedRouteRules.Add(routeRule);

                        if (!routeRule.IsEffective(effectiveDate) && routeRule.IsEffective(partialRouteInfo.LatestRoutingDate))
                            affectedRouteRules.ClosedRouteRules.Add(routeRule);
                    }
                }
            }

            this.AffectedRouteRules.Set(context, affectedRouteRules);
        }
    }
}