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

        [RequiredArgument]
        public OutArgument<AffectedRouteOptionRules> AffectedRouteOptionRules { get; set; }

        [RequiredArgument]
        public OutArgument<DateTime?> NextOpenOrCloseRuleTime { get; set; }

        protected override void Execute(CodeActivityContext context)
        {
            DateTime effectiveDate = this.EffectiveDate.Get(context);
            PartialRouteInfo partialRouteInfo = this.PartialRouteInfo.Get(context);

            int? routeRuleId = this.RouteRuleId.Get(context);
            DateTime? nextOpenOrCloseRuleTime = null;

            RouteRuleManager routeRuleManager = new RouteRuleManager();
            RouteOptionRuleManager routeOptionRuleManager = new RouteOptionRuleManager();

            List<RuleChangedData<RouteRule>> routeRuleChangedList;
            List<RuleChangedData<RouteOptionRule>> routeOptionRuleChangedList = null;

            if (routeRuleId.HasValue)
            {
                routeRuleChangedList = new List<RuleChangedData<RouteRule>>() { routeRuleManager.FillAndGetRuleChangedForProcessing(routeRuleId.Value) };
            }
            else
            {
                routeRuleChangedList = routeRuleManager.FillAndGetRulesChangedForProcessing();
                routeOptionRuleChangedList = routeOptionRuleManager.FillAndGetRulesChangedForProcessing();
            }

            AffectedRouteRules affectedRouteRules = new AffectedRouteRules();
            HashSet<int> affectedRouteRuleIds = new HashSet<int>();
            BuildAddedUpdatedRouteRules(routeRuleChangedList, affectedRouteRules, affectedRouteRuleIds, effectiveDate);

            AffectedRouteOptionRules affectedRouteOptionRules = new AffectedRouteOptionRules();
            HashSet<int> affectedRouteOptionRuleIds = new HashSet<int>();
            BuildAddedUpdatedRouteOptionRules(routeOptionRuleChangedList, affectedRouteOptionRules, affectedRouteOptionRuleIds, effectiveDate);


            if (!routeRuleId.HasValue && effectiveDate > partialRouteInfo.LatestRoutingDate)
            {
                BuildOpenedClosedRouteRules(affectedRouteRules, affectedRouteRuleIds, effectiveDate, partialRouteInfo, ref nextOpenOrCloseRuleTime);
                BuildOpenedClosedRouteOptionRules(affectedRouteOptionRules, affectedRouteOptionRuleIds, effectiveDate, partialRouteInfo, ref nextOpenOrCloseRuleTime);
            }

            this.AffectedRouteRules.Set(context, affectedRouteRules);
            this.AffectedRouteOptionRules.Set(context, affectedRouteOptionRules);
            this.NextOpenOrCloseRuleTime.Set(context, nextOpenOrCloseRuleTime);
        }

        void BuildAddedUpdatedRouteRules(List<RuleChangedData<RouteRule>> routeRuleChangedList, AffectedRouteRules affectedRouteRules, HashSet<int> affectedRouteRuleIds, DateTime effectiveDate)
        {
            if (routeRuleChangedList != null)
            {
                RouteRuleManager routeRuleManager = new RouteRuleManager();
                foreach (var routeRuleChanged in routeRuleChangedList)
                {
                    switch (routeRuleChanged.ActionType)
                    {
                        case ActionType.AddedRule:
                            affectedRouteRuleIds.Add(routeRuleChanged.RuleId);
                            RouteRule addedRouteRule = routeRuleManager.GetRule(routeRuleChanged.RuleId);

                            if (addedRouteRule.IsEffective(effectiveDate))
                                affectedRouteRules.AddedRouteRules.Add(addedRouteRule);
                            break;

                        case ActionType.UpdatedRule:
                            affectedRouteRules.UpdatedRouteRules.Add(routeRuleChanged.InitialRule);

                            RouteRuleAdditionalInformation routeRuleAdditionalInformation = routeRuleChanged.AdditionalInformation.CastWithValidate<RouteRuleAdditionalInformation>("routeRuleChanged.AdditionalInformation", routeRuleChanged.RuleId);
                            if (routeRuleAdditionalInformation.CriteriaHasChanged)
                            {
                                affectedRouteRuleIds.Add(routeRuleChanged.RuleId);
                                RouteRule routeRule = routeRuleManager.GetRule(routeRuleChanged.RuleId);
                                affectedRouteRules.UpdatedRouteRules.Add(routeRule);
                            }
                            break;

                        default: throw new NotSupportedException(string.Format("ActionType {0} not supported.", routeRuleChanged.ActionType));
                    }
                }
            }
        }

        void BuildAddedUpdatedRouteOptionRules(List<RuleChangedData<RouteOptionRule>> routeOptionRuleChangedList, AffectedRouteOptionRules affectedRouteOptionRules, HashSet<int> affectedRouteOptionRuleIds, DateTime effectiveDate)
        {
            if (routeOptionRuleChangedList != null)
            {
                RouteOptionRuleManager routeOptionRuleManager = new RouteOptionRuleManager();
                foreach (var routeOptionRuleChanged in routeOptionRuleChangedList)
                {
                    switch (routeOptionRuleChanged.ActionType)
                    {
                        case ActionType.AddedRule:
                            affectedRouteOptionRuleIds.Add(routeOptionRuleChanged.RuleId);
                            RouteOptionRule addedRouteOptionRule = routeOptionRuleManager.GetRule(routeOptionRuleChanged.RuleId);

                            if (addedRouteOptionRule.IsEffective(effectiveDate))
                                affectedRouteOptionRules.AddedRouteOptionRules.Add(addedRouteOptionRule);
                            break;

                        case ActionType.UpdatedRule:
                            affectedRouteOptionRules.UpdatedRouteOptionRules.Add(routeOptionRuleChanged.InitialRule);

                            RouteOptionRuleAdditionalInformation routeOptionRuleAdditionalInformation = routeOptionRuleChanged.AdditionalInformation.CastWithValidate<RouteOptionRuleAdditionalInformation>("routeOptionRuleChanged.AdditionalInformation", routeOptionRuleChanged.RuleId);
                            if (routeOptionRuleAdditionalInformation.CriteriaHasChanged)
                            {
                                affectedRouteOptionRuleIds.Add(routeOptionRuleChanged.RuleId);
                                RouteOptionRule routeOptionRule = routeOptionRuleManager.GetRule(routeOptionRuleChanged.RuleId);
                                affectedRouteOptionRules.UpdatedRouteOptionRules.Add(routeOptionRule);
                            }
                            break;

                        default: throw new NotSupportedException(string.Format("ActionType {0} not supported.", routeOptionRuleChanged.ActionType));
                    }
                }
            }
        }

        void BuildOpenedClosedRouteRules(AffectedRouteRules affectedRouteRules, HashSet<int> affectedRouteRuleIds, DateTime effectiveDate, PartialRouteInfo partialRouteInfo, ref DateTime? nextOpenOrCloseRuleTime)
        {
            Dictionary<int, RouteRule> routeRules = new RouteRuleManager().GetAllRules();
            if (routeRules != null)
            {
                foreach (var routeRuleKvp in routeRules)
                {
                    int ruleId = routeRuleKvp.Key;
                    RouteRule routeRule = routeRuleKvp.Value;

                    if (routeRule.BED > effectiveDate && (!nextOpenOrCloseRuleTime.HasValue || routeRule.BED < nextOpenOrCloseRuleTime))
                        nextOpenOrCloseRuleTime = routeRule.BED;

                    if (routeRule.EED.HasValue && routeRule.EED > effectiveDate && (!nextOpenOrCloseRuleTime.HasValue || routeRule.EED < nextOpenOrCloseRuleTime.Value))
                        nextOpenOrCloseRuleTime = routeRule.EED;


                    if (affectedRouteRuleIds.Contains(ruleId))
                        continue;

                    if (routeRule.IsEffective(effectiveDate) && !routeRule.IsEffective(partialRouteInfo.LatestRoutingDate))
                        affectedRouteRules.OpenedRouteRules.Add(routeRule);

                    if (!routeRule.IsEffective(effectiveDate) && routeRule.IsEffective(partialRouteInfo.LatestRoutingDate))
                        affectedRouteRules.ClosedRouteRules.Add(routeRule);
                }
            }
        }

        void BuildOpenedClosedRouteOptionRules(AffectedRouteOptionRules affectedRouteOptionRules, HashSet<int> affectedRouteOptionRuleIds, DateTime effectiveDate, PartialRouteInfo partialRouteInfo, ref DateTime? nextOpenOrCloseRuleTime)
        {
            Dictionary<int, RouteOptionRule> routeOptionRules = new RouteOptionRuleManager().GetAllRules();
            if (routeOptionRules != null)
            {
                foreach (var routeOptionRuleKvp in routeOptionRules)
                {
                    int ruleId = routeOptionRuleKvp.Key;
                    RouteOptionRule routeOptionRule = routeOptionRuleKvp.Value;

                    if (routeOptionRule.BED > effectiveDate && (!nextOpenOrCloseRuleTime.HasValue || routeOptionRule.BED < nextOpenOrCloseRuleTime))
                        nextOpenOrCloseRuleTime = routeOptionRule.BED;

                    if (routeOptionRule.EED.HasValue && routeOptionRule.EED > effectiveDate && (!nextOpenOrCloseRuleTime.HasValue || routeOptionRule.EED < nextOpenOrCloseRuleTime.Value))
                        nextOpenOrCloseRuleTime = routeOptionRule.EED;

                    if (affectedRouteOptionRuleIds.Contains(ruleId))
                        continue;

                    if (routeOptionRule.IsEffective(effectiveDate) && !routeOptionRule.IsEffective(partialRouteInfo.LatestRoutingDate))
                        affectedRouteOptionRules.OpenedRouteOptionRules.Add(routeOptionRule);

                    if (!routeOptionRule.IsEffective(effectiveDate) && routeOptionRule.IsEffective(partialRouteInfo.LatestRoutingDate))
                        affectedRouteOptionRules.ClosedRouteOptionRules.Add(routeOptionRule);
                }
            }
        }
    }
}