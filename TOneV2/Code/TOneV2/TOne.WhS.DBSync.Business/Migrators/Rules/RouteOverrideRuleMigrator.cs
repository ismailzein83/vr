﻿using System;
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

namespace TOne.WhS.DBSync.Business
{
    public class RouteOverrideRuleMigrator : RouteRuleBaseMigrator
    {
        readonly Dictionary<string, CarrierAccount> _allCarrierAccounts;
        readonly Dictionary<string, SaleZone> _allSaleZones;
        public RouteOverrideRuleMigrator(RuleMigrationContext context)
            : base(context)
        {
            var dbTableCarrierAccount = Context.MigrationContext.DBTables[DBTableName.CarrierAccount];
            _allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;

            var dbTableSaleZones = Context.MigrationContext.DBTables[DBTableName.SaleZone];
            _allSaleZones = (Dictionary<string, SaleZone>)dbTableSaleZones.Records;
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

            return new SourceRule
            {
                RouteRule = new RouteRule
                {
                    BeginEffectiveTime = rules.Min(r => r.BED),
                    EndEffectiveTime = null,
                    Description = sourceRule.Reason,
                    Name = string.IsNullOrEmpty(sourceRule.Reason) ? "Migrated Rule" : sourceRule.Reason,
                    Criteria = GetRuleCodeCriteria(GetRuleCodeCriterias(rules), sourceRule),
                    Settings = GetRuleSettings(sourceRule)

                }
            };
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
                    ConfigId = SelectiveCodeCriteriaGroup.ExtensionConfigId
                },
                CustomerGroupSettings = new SelectiveCustomerGroup
                {
                    CustomerIds = new List<int>() { _allCarrierAccounts[sourceRule.CustomerId].CarrierAccountId },
                    ConfigId = SelectiveCustomerGroup.ExtensionConfigId
                }
            };
        }
        SourceRule GetSourceRuleFromZones(IEnumerable<SourceRouteOverrideRule> rules)
        {
            SourceRouteOverrideRule sourceRule = rules.First();

            List<long> lstZoneIds = new List<long>();

            foreach (var rule in rules)
                lstZoneIds.Add(_allSaleZones[rule.SaleZoneId.ToString()].SaleZoneId);

            return new SourceRule
            {
                RouteRule = new RouteRule
                {
                    BeginEffectiveTime = rules.Min(r => r.BED),
                    EndEffectiveTime = null,
                    Description = sourceRule.Reason,
                    Name = string.IsNullOrEmpty(sourceRule.Reason) ? "Migrated Rule" : sourceRule.Reason,
                    Criteria = GetRuleZoneCriteria(lstZoneIds, sourceRule),
                    Settings = GetRuleSettings(sourceRule)
                }
            };
        }
        RegularRouteRule GetRuleSettings(SourceRouteOverrideRule sourceRule)
        {
            return new RegularRouteRule
            {

                OptionsSettingsGroup = new SelectiveOptions
                {
                    Options = GetOptions(sourceRule),
                    ConfigId = SelectiveOptions.ExtensionConfigId
                },
                OptionPercentageSettings = GetOptionPercentageSettings(sourceRule),
                ConfigId = RegularRouteRule.ExtensionConfigId

            };
        }
        RouteRuleCriteria GetRuleZoneCriteria(List<long> lstZoneIds, SourceRouteOverrideRule sourceRule)
        {
            return new RouteRuleCriteria
            {
                SaleZoneGroupSettings = new SelectiveSaleZoneGroup { ZoneIds = lstZoneIds, ConfigId = SelectiveSaleZoneGroup.ExtensionConfigId, SellingNumberPlanId = Context.MigrationContext.DefaultSellingNumberPlanId },
                CustomerGroupSettings = new SelectiveCustomerGroup
                {
                    CustomerIds = new List<int>() { _allCarrierAccounts[sourceRule.CustomerId].CarrierAccountId },
                    ConfigId = SelectiveCustomerGroup.ExtensionConfigId
                }
            };
        }
        FixedOptionPercentage GetOptionPercentageSettings(SourceRouteOverrideRule sourceRule)
        {
            if (sourceRule.SupplierOptions.Sum(s => s.Percentage) != (short)100)
                return null;

            FixedOptionPercentage setting = new FixedOptionPercentage { Percentages = new List<decimal>(), ConfigId = FixedOptionPercentage.ExtensionConfigId };
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
