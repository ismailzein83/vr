﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public class RouteManager
    {
        public void ConvertRouteOptions(List<RouteSupplierOption> options, out string suppliersOption, out string suppliersOrderOption, out string percentagesOption)
        {
            suppliersOption = null;
            suppliersOrderOption = null;
            percentagesOption = null;
            if(options != null && options.Count > 0)
            {
                var orderedOptions = options.OrderBy(itm => itm.SupplierId).ToList();
                StringBuilder suppliersOptionBuilder = new StringBuilder();
                
                bool isDefaultOrder = true;
                bool isDefaultPercentage = true;
                int index = 0;
                foreach(var option in orderedOptions)
                {
                    if (suppliersOptionBuilder == null)
                        suppliersOptionBuilder = new StringBuilder();
                    else
                        suppliersOptionBuilder.Append(',');
                    suppliersOptionBuilder.Append(option.SupplierId);

                    if (options[index] != option)
                        isDefaultOrder = false;
                    if (option.Percentage.HasValue)
                        isDefaultPercentage = false;
                    index++;
                }
                suppliersOption = suppliersOptionBuilder.ToString();

                if (!isDefaultOrder || !isDefaultPercentage)
                {
                    StringBuilder suppliersOrderOptionBuilder = null;
                    StringBuilder percentagesOptionBuilder = null;
                    foreach(var option in options)
                    {
                        if(!isDefaultOrder)
                        {
                            if (suppliersOrderOptionBuilder == null)
                                suppliersOrderOptionBuilder = new StringBuilder();
                            else
                                suppliersOrderOptionBuilder.Append(',');
                            suppliersOrderOptionBuilder.Append(orderedOptions.IndexOf(option));
                        }
                        if (!isDefaultPercentage)
                        {
                            if (percentagesOptionBuilder == null)
                                percentagesOptionBuilder = new StringBuilder();
                            else
                                percentagesOptionBuilder.Append(',');
                            percentagesOptionBuilder.Append(option.Percentage);
                        }
                    }
                    if (!isDefaultOrder)
                        suppliersOrderOption = suppliersOrderOptionBuilder.ToString();
                    if (!isDefaultPercentage)
                        percentagesOption = percentagesOptionBuilder.ToString();
                }
            }
        }

        public void StructureRulesForRouteBuild(List<RouteRule> rules, out RouteRulesByActionDataType routeRules, out RouteOptionRulesBySupplier routeOptionRules)
        {
            routeRules = new RouteRulesByActionDataType();
            routeOptionRules = new RouteOptionRulesBySupplier();
            foreach (var rule in rules)
            {
                if (rule.ActionData == null)
                    continue;
                RouteRulesByActionDataType targetRules;
                if (rule.Type == RouteRuleType.RouteRule)
                    targetRules = routeRules;
                else
                {
                    SupplierSelectionSet supplierSelectionSet = rule.CarrierAccountSet as SupplierSelectionSet;
                    if (supplierSelectionSet == null)
                    {
                        SetRuleInvalid(rule);
                        continue;
                    }

                    if (!routeOptionRules.Rules.TryGetValue(supplierSelectionSet.SupplierId, out targetRules))
                    {
                        targetRules = new RouteRulesByActionDataType();
                        routeOptionRules.Rules.Add(supplierSelectionSet.SupplierId, targetRules);
                    }
                }

                Type ruleActionDataType = rule.ActionData.GetType();
                RouteRuleMatches matches;
                if (!targetRules.Rules.TryGetValue(ruleActionDataType, out matches))
                {
                    matches = new RouteRuleMatches();
                    targetRules.Rules.Add(ruleActionDataType, matches);
                }
                AddRuleMatches(matches, rule);
            }

            foreach (var ruleMatch in routeRules.Rules.Values)
            {
                ruleMatch.SetMinSubCodeLength();
            }
            foreach (var supplierRules in routeOptionRules.Rules.Values)
            {
                foreach (var ruleMatch in supplierRules.Rules.Values)
                {
                    ruleMatch.SetMinSubCodeLength();
                }
            }
        }

        private void SetRuleInvalid(RouteRule rule)
        {
            throw new NotImplementedException();
        }         

        private void AddRuleMatches(RouteRuleMatches ruleMatches, RouteRule rule)
        {
            CodeSetMatch codeSetMatch = rule.CodeSet.GetMatch();
            if(codeSetMatch.MatchCodes != null)
            {
                foreach (var matchCodeEntry in codeSetMatch.MatchCodes)
                {
                    Dictionary<string, List<RouteRule>> codeRules = matchCodeEntry.Value ? ruleMatches.RulesByMatchCodeAndSubCodes : ruleMatches.RulesByMatchCodes;
                    AddRuleToGroupDictionary(codeRules, matchCodeEntry.Key, rule);
                }
            }

            if (codeSetMatch.IsMatchingAllZones)
                ruleMatches.RulesMatchingAllZones.Add(rule);
            else
                if(codeSetMatch.MatchZoneIds!= null)
                {
                    foreach(int matchZoneId in codeSetMatch.MatchZoneIds)
                    {
                        AddRuleToGroupDictionary(ruleMatches.RulesByMatchZones, matchZoneId, rule);
                    }
                }
        }

        void AddRuleToGroupDictionary<T>(Dictionary<T, List<RouteRule>> dictionary, T key, RouteRule rule)
        {
            List<RouteRule> routeRules;
            if (!dictionary.TryGetValue(key, out routeRules))
            {
                routeRules = new List<RouteRule>();
                dictionary.Add(key, routeRules);
            }
            routeRules.Add(rule);
        }
    }

    public class RouteRulesByActionDataType
    {
        public RouteRulesByActionDataType()
        {
            this.Rules = new Dictionary<Type, RouteRuleMatches>();
        }

        public Dictionary<Type, RouteRuleMatches> Rules { get; private set; }
    }

    public class RouteOptionRulesBySupplier
    {
        public RouteOptionRulesBySupplier()
        {
            Rules = new Dictionary<string, RouteRulesByActionDataType>();
        }
        public Dictionary<string, RouteRulesByActionDataType> Rules { get; private set; }
    }
}
