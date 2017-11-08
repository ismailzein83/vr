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
        public InArgument<int?> RouteRuleId { get; set; }


        [RequiredArgument]
        public OutArgument<AffectedRouteRules> AffectedRouteRules { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
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
                UpdatedRouteRules = new List<RouteRule>()
            };

            if (ruleChangedList != null)
            {
                foreach (var ruleChanged in ruleChangedList)
                {
                    switch (ruleChanged.ActionType)
                    {
                        case ActionType.AddedRule: affectedRouteRules.AddedRouteRules.Add(ruleChanged.InitialRule); break;
                        case ActionType.UpdatedRule:
                            affectedRouteRules.UpdatedRouteRules = new List<RouteRule>() { ruleChanged.InitialRule };
                            RouteRuleAdditionalInformation routeRuleAdditionalInformation = ruleChanged.AdditionalInformation.CastWithValidate<RouteRuleAdditionalInformation>("ruleChanged.AdditionalInformation", ruleChanged.RuleId);
                            if (routeRuleAdditionalInformation.CriteriaHasChanged)
                            {
                                RouteRule routeRule = routeRuleManager.GetRouteRule(ruleChanged.RuleId);
                                affectedRouteRules.UpdatedRouteRules.Add(routeRule);
                            }
                            break;

                        default: throw new NotSupportedException(string.Format("ActionType {0} not supported.", ruleChanged.ActionType));
                    }
                }
            }
            this.AffectedRouteRules.Set(context, affectedRouteRules);
        }
    }
}