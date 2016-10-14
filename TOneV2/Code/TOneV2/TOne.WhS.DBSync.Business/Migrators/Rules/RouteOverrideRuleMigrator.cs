using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.MainExtensions.CodeCriteriaGroups;
using TOne.WhS.BusinessEntity.MainExtensions.CustomerGroups;
using TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups;
using TOne.WhS.DBSync.Data.SQL.SourceDataManger;
using TOne.WhS.DBSync.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Business.RouteRules.Filters;
using TOne.WhS.Routing.Business.RouteRules.OptionSettingsGroups;
using TOne.WhS.Routing.Business.RouteRules.Percentages;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Rules.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class RouteOverrideRuleMigrator : RuleBaseMigrator
    {
        public override string EntityName
        {
            get { return "Route Override"; }
        }

        readonly Dictionary<string, CarrierAccount> _allCarrierAccounts;
        readonly Dictionary<string, SaleZone> _allSaleZones;
        readonly int _routeRuleTypeId;
        public RouteOverrideRuleMigrator(RuleMigrationContext context)
            : base(context)
        {
            var dbTableCarrierAccount = Context.MigrationContext.DBTables[DBTableName.CarrierAccount];
            _allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;

            var dbTableSaleZones = Context.MigrationContext.DBTables[DBTableName.SaleZone];
            _allSaleZones = (Dictionary<string, SaleZone>)dbTableSaleZones.Records;

            RouteRuleManager manager = new RouteRuleManager();
            _routeRuleTypeId = manager.GetRuleTypeId();
        }
        public override IEnumerable<SourceRule> GetSourceRules()
        {
            List<SourceRule> routeRules = new List<SourceRule>();

            SourceRouteOverrideRuleDataManager dataManager = new SourceRouteOverrideRuleDataManager(Context.MigrationContext.ConnectionString, true);
            var overrideRules = dataManager.GetRouteOverrideRules();

            routeRules.AddRange(GetRulesWithCode(overrideRules.Where(o => !o.SaleZoneId.HasValue)));
            routeRules.AddRange(GetRulesWithZone(overrideRules.Where(o => o.SaleZoneId.HasValue)));

            return routeRules;
        }
        IEnumerable<SourceRule> GetRulesWithZone(IEnumerable<SourceRouteOverrideRule> overrideRules)
        {
            List<SourceRule> routeRules = new List<SourceRule>();
            var dicRules = GetRulesDictionary(overrideRules);
            foreach (var rules in dicRules.Values)
            {
                SourceRouteOverrideRule sourceRule = rules.First();
                if (sourceRule == null)
                    continue;
                var rule = GetSourceRuleFromZones(rules);
                if (rule == null)
                {
                    this.TotalRowsFailed++;
                }
                else
                {
                    routeRules.Add(rule);
                }
            }
            return routeRules;

        }
        Dictionary<string, List<SourceRouteOverrideRule>> GetRulesDictionary(IEnumerable<SourceRouteOverrideRule> overrideRules)
        {
            Dictionary<string, List<SourceRouteOverrideRule>> dicRules = new Dictionary<string, List<SourceRouteOverrideRule>>();
            foreach (var routeRule in overrideRules)
            {
                string key = string.Format("{0},{1},{2},{3}", routeRule.CustomerId,
                    routeRule.SupplierOptions.Select(s => s.ToString()).Aggregate((i, j) => i + j), routeRule.BED, routeRule.EED);

                List<SourceRouteOverrideRule> lstRules;
                if (!dicRules.TryGetValue(key, out lstRules))
                {
                    lstRules = new List<SourceRouteOverrideRule>();
                    dicRules.Add(key, lstRules);
                }
                lstRules.Add(routeRule);
            }
            return dicRules;
        }
        IEnumerable<SourceRule> GetRulesWithCode(IEnumerable<SourceRouteOverrideRule> overrideRules)
        {
            List<SourceRule> routeRules = new List<SourceRule>();
            var dicRules = GetRulesDictionary(overrideRules);
            foreach (var rules in dicRules.Values)
            {
                SourceRouteOverrideRule sourceRule = rules.First();
                if (sourceRule == null)
                    continue;
                var rule = GetSourceRuleFromCodes(rules);
                if (rule == null)
                    this.TotalRowsFailed++;
                else
                    routeRules.Add(rule);                
            }
            return routeRules;
        }
        SourceRule GetSourceRuleFromCodes(IEnumerable<SourceRouteOverrideRule> rules)
        {
            SourceRouteOverrideRule sourceRule = rules.First();
            var details = GetRuleDetailsFromCode(rules, sourceRule);
            if (details == null)
            {
                return null;
            }
            else
            {
                return new SourceRule
                {
                    Rule = new Rule
                    {
                        BED = sourceRule.BED,
                        EED = sourceRule.EED,
                        RuleDetails = Serializer.Serialize(details),
                        TypeId = _routeRuleTypeId
                    }
                };
            }
        }

        RouteRule GetRuleDetailsFromCode(IEnumerable<SourceRouteOverrideRule> rules, SourceRouteOverrideRule sourceRule)
        {
            var criteria = GetRuleCodeCriteria(GetRuleCodeCriterias(rules), sourceRule);
            if (criteria == null)
                return null;
            else
            {
                RouteRule details = new RouteRule
                {
                    BeginEffectiveTime = sourceRule.BED,
                    EndEffectiveTime = sourceRule.EED,
                    Description = sourceRule.Reason,
                    Name = string.IsNullOrEmpty(sourceRule.Reason) ? "Migrated Rule" : sourceRule.Reason,
                    Criteria = criteria,
                    Settings = GetRuleSettings(sourceRule)
                };
                return details;
            }
        }

        List<CodeCriteria> GetRuleCodeCriterias(IEnumerable<SourceRouteOverrideRule> rules)
        {
            List<CodeCriteria> criterias = new List<CodeCriteria>();

            foreach (var sourceRouteOverrideRule in rules)
            {
                CodeCriteria codeCriteria = new CodeCriteria
                {
                    Code = sourceRouteOverrideRule.Code,
                    WithSubCodes = sourceRouteOverrideRule.IncludeSubCode
                };
                criterias.Add(codeCriteria);
            }
            return criterias;
        }
        RouteRuleCriteria GetRuleCodeCriteria(List<CodeCriteria> codeCriterias, SourceRouteOverrideRule sourceRule)
        {
            CarrierAccount customer;
            if (!_allCarrierAccounts.TryGetValue(sourceRule.CustomerId, out customer))
            {
                return null;
            }
            else
            {
                return new RouteRuleCriteria
                {
                    CodeCriteriaGroupSettings = new SelectiveCodeCriteriaGroup
                    {
                        Codes = codeCriterias,
                    },
                    CustomerGroupSettings = new SelectiveCustomerGroup
                    {
                        CustomerIds = new List<int>() { customer.CarrierAccountId },
                    }
                };
            }
        }
        SourceRule GetSourceRuleFromZones(IEnumerable<SourceRouteOverrideRule> rules)
        {
            SourceRouteOverrideRule sourceRule = rules.First();

            List<long> lstZoneIds = new List<long>();

            foreach (var rule in rules)
                if (!_allSaleZones.ContainsKey(rule.SaleZoneId.ToString()))
                    this.TotalRowsFailed++;
                else
                    lstZoneIds.Add(_allSaleZones[rule.SaleZoneId.ToString()].SaleZoneId);

            var ruleDetails = GetRuleDetailsFromZone(rules, sourceRule, lstZoneIds);
            if (ruleDetails == null)
                return null;
            else
            {
                return new SourceRule
                {
                    Rule = new Rule
                    {
                        BED = sourceRule.BED,
                        EED = sourceRule.EED,
                        TypeId = _routeRuleTypeId,
                        RuleDetails = Serializer.Serialize(ruleDetails)
                    }
                };
            }
        }

        private RouteRule GetRuleDetailsFromZone(IEnumerable<SourceRouteOverrideRule> rules, SourceRouteOverrideRule sourceRule, List<long> lstZoneIds)
        {
            var criteria = GetRuleZoneCriteria(lstZoneIds, sourceRule);
            if (criteria == null)
                return null;
            else
            {
                RouteRule details = new RouteRule
                {
                    BeginEffectiveTime = sourceRule.BED,
                    EndEffectiveTime = sourceRule.EED,
                    Description = sourceRule.Reason,
                    Name = string.IsNullOrEmpty(sourceRule.Reason) ? "Migrated Rule" : sourceRule.Reason,
                    Criteria = criteria,
                    Settings = GetRuleSettings(sourceRule)
                };
                return details;
            }
        }

        RegularRouteRule GetRuleSettings(SourceRouteOverrideRule sourceRule)
        {
            var rule = new RegularRouteRule
            {

                OptionsSettingsGroup = new SelectiveOptions
                {
                    Options = GetOptions(sourceRule)
                },
                OptionPercentageSettings = GetOptionPercentageSettings(sourceRule)
            };              
            //if (!sourceRule.SupplierOptions.Any(r => r.IsLoss))
            //{
            //    rule.OptionFilters = new List<RouteOptionFilterSettings> { 
            //    new RateOptionFilter
            //        {
            //            RateOption = RateOption.MaximumLoss,
            //            RateOptionType = RateOptionType.Fixed,
            //            RateOptionValue = 0
            //        }
            //    };
            //}
            return rule;
        }
        RouteRuleCriteria GetRuleZoneCriteria(List<long> lstZoneIds, SourceRouteOverrideRule sourceRule)
        {
            CarrierAccount customer;
            if (!_allCarrierAccounts.TryGetValue(sourceRule.CustomerId, out customer))
            {
                return null;
            }
            else
            {
                return new RouteRuleCriteria
                {
                    SaleZoneGroupSettings = new SelectiveSaleZoneGroup { ZoneIds = lstZoneIds, SellingNumberPlanId = Context.MigrationContext.DefaultSellingNumberPlanId },
                    CustomerGroupSettings = new SelectiveCustomerGroup
                    {
                        CustomerIds = new List<int>() { customer.CarrierAccountId },
                    }
                };
            }
        }
        FixedOptionPercentage GetOptionPercentageSettings(SourceRouteOverrideRule sourceRule)
        {
            if (sourceRule.SupplierOptions.Sum(s => s.Percentage) != (short)100)
                return null;

            FixedOptionPercentage setting = new FixedOptionPercentage { Percentages = new List<decimal>() };
            foreach (var option in sourceRule.SupplierOptions)
                setting.Percentages.Add(option.Percentage);
            return setting;
        }
        List<RouteOptionSettings> GetOptions(SourceRouteOverrideRule sourceRule)
        {
            return sourceRule.SupplierOptions.Select(option => new RouteOptionSettings
            {
                SupplierId = _allCarrierAccounts[option.SupplierId].CarrierAccountId,
                Percentage = (decimal?)option.Percentage
            }).ToList();
        }
    }
}
