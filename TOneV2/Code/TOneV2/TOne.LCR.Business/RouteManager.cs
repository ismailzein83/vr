using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public class RouteManager
    {
        public void ConvertRouteOptions(List<SupplierRoute> options, out string suppliersOption, out string suppliersOrderOption, out string percentagesOption)
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

        public void ManipulateRouteRules(List<BaseRouteRule> rules, out RouteRuleMatches saleRuleMatches, out RouteRuleMatches supplierRuleMatches)
        {
            saleRuleMatches = new RouteRuleMatches();
            supplierRuleMatches = new RouteRuleMatches();
            foreach(var rule in rules)
            {
                if (rule is SupplierRouteRule)
                    AddRuleMatches(supplierRuleMatches, rule);
                else
                    AddRuleMatches(saleRuleMatches, rule);
            }
        }

        private void AddRuleMatches(RouteRuleMatches ruleMatches, BaseRouteRule rule)
        {
            CodeSetMatch codeSetMatch = rule.CodeSet.GetMatch();
            if(codeSetMatch.MatchCodes != null)
            {
                foreach (var matchCodeEntry in codeSetMatch.MatchCodes)
                {
                    Dictionary<string, List<BaseRouteRule>> codeRules = matchCodeEntry.Value ? ruleMatches.RulesByMatchCodeAndSubCodes : ruleMatches.RulesByMatchCodes;
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

            CarrierAccountSetMatch carrierAccountSetMatch = rule.CarrierAccountSet.GetMatch();
            if (carrierAccountSetMatch.IsMatchingAllAccounts)
                ruleMatches.RulesMatchingAllCarrierAccounts.Add(rule);
            else
                if(carrierAccountSetMatch.MatchAccountIds != null)
                {
                    foreach(string matchAccountId in carrierAccountSetMatch.MatchAccountIds)
                    {
                        AddRuleToGroupDictionary(ruleMatches.RulesByMatchCarrierAccounts, matchAccountId, rule);
                    }
                }
        }

        void AddRuleToGroupDictionary<T>(Dictionary<T, List<BaseRouteRule>> dictionary, T key, BaseRouteRule rule)
        {
            List<BaseRouteRule> routeRules;
            if (!dictionary.TryGetValue(key, out routeRules))
            {
                routeRules = new List<BaseRouteRule>();
                dictionary.Add(key, routeRules);
            }
            routeRules.Add(rule);
        }
    }
}
