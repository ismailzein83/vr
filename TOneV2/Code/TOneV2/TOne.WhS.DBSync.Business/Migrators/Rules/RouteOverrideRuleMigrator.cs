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
using TOne.WhS.Routing.Business.RouteRules.OptionSettingsGroups;
using TOne.WhS.Routing.Business.RouteRules.Percentages;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Rules.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class RouteOverrideRuleMigrator : RouteRuleBaseMigrator
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
        public override IEnumerable<SourceRule> GetRouteRules()
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
                routeRules.Add(GetSourceRuleFromZones(rules));
            }
            return routeRules;

        }
        Dictionary<string, List<SourceRouteOverrideRule>> GetRulesDictionary(IEnumerable<SourceRouteOverrideRule> overrideRules)
        {
            Dictionary<string, List<SourceRouteOverrideRule>> dicRules = new Dictionary<string, List<SourceRouteOverrideRule>>();
            foreach (var routeRule in overrideRules)
            {
                string key = string.Format("{0},{1}", routeRule.CustomerId,
                    routeRule.SupplierOptions.Select(s => s.ToString()).Aggregate((i, j) => i + j));

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
                routeRules.Add(GetSourceRuleFromCodes(rules));
            }
            return routeRules;
        }
        SourceRule GetSourceRuleFromCodes(IEnumerable<SourceRouteOverrideRule> rules)
        {
            SourceRouteOverrideRule sourceRule = rules.First();
            var details = GetRuleDetailsFromCode(rules, sourceRule);
            return new SourceRule
            {
                Rule = new Rule
                {
                    BED = rules.Min(r => r.BED),
                    EED = null,
                    RuleDetails = Serializer.Serialize(details),
                    TypeId = _routeRuleTypeId
                }
            };
        }

        RouteRule GetRuleDetailsFromCode(IEnumerable<SourceRouteOverrideRule> rules, SourceRouteOverrideRule sourceRule)
        {
            RouteRule details = new RouteRule
            {
                BeginEffectiveTime = rules.Min(r => r.BED),
                EndEffectiveTime = null,
                Description = sourceRule.Reason,
                Name = string.IsNullOrEmpty(sourceRule.Reason) ? "Migrated Rule" : sourceRule.Reason,
                Criteria = GetRuleCodeCriteria(GetRuleCodeCriterias(rules), sourceRule),
                Settings = GetRuleSettings(sourceRule)
            };
            return details;
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
            return new RouteRuleCriteria
            {
                CodeCriteriaGroupSettings = new SelectiveCodeCriteriaGroup
                {
                    Codes = codeCriterias,
                },
                CustomerGroupSettings = new SelectiveCustomerGroup
                {
                    CustomerIds = new List<int>() { _allCarrierAccounts[sourceRule.CustomerId].CarrierAccountId },
                }
            };
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

            return new SourceRule
            {
                Rule = new Rule
                {
                    BED = rules.Min(r => r.BED),
                    EED = null,
                    TypeId = _routeRuleTypeId,
                    RuleDetails = Serializer.Serialize(GetRuleDetailsFromZone(rules, sourceRule, lstZoneIds))
                }
            };
        }

        private RouteRule GetRuleDetailsFromZone(IEnumerable<SourceRouteOverrideRule> rules, SourceRouteOverrideRule sourceRule, List<long> lstZoneIds)
        {
            RouteRule details = new RouteRule
            {
                BeginEffectiveTime = rules.Min(r => r.BED),
                EndEffectiveTime = null,
                Description = sourceRule.Reason,
                Name = string.IsNullOrEmpty(sourceRule.Reason) ? "Migrated Rule" : sourceRule.Reason,
                Criteria = GetRuleZoneCriteria(lstZoneIds, sourceRule),
                Settings = GetRuleSettings(sourceRule)
            };
            return details;
        }

        RegularRouteRule GetRuleSettings(SourceRouteOverrideRule sourceRule)
        {
            return new RegularRouteRule
            {

                OptionsSettingsGroup = new SelectiveOptions
                {
                    Options = GetOptions(sourceRule),
                },
                OptionPercentageSettings = GetOptionPercentageSettings(sourceRule),
            };
        }
        RouteRuleCriteria GetRuleZoneCriteria(List<long> lstZoneIds, SourceRouteOverrideRule sourceRule)
        {
            return new RouteRuleCriteria
            {
                SaleZoneGroupSettings = new SelectiveSaleZoneGroup { ZoneIds = lstZoneIds, SellingNumberPlanId = Context.MigrationContext.DefaultSellingNumberPlanId },
                CustomerGroupSettings = new SelectiveCustomerGroup
                {
                    CustomerIds = new List<int>() { _allCarrierAccounts[sourceRule.CustomerId].CarrierAccountId },
                }
            };
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
