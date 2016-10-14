using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.MainExtensions.CodeCriteriaGroups;
using TOne.WhS.BusinessEntity.MainExtensions.CustomerGroups;
using TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups;
using TOne.WhS.BusinessEntity.MainExtensions.SuppliersWithZonesGroups;
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
    public class RouteOptionBlockRuleMigrator : RuleBaseMigrator
    {
        public override string EntityName
        {
            get { return "Route Option Block"; }
        }

        readonly Dictionary<string, CarrierAccount> _allCarrierAccounts;
        readonly Dictionary<string, SupplierZone> _allSupplierZones;
        readonly int _routeOptionRuleTypeId;
        public RouteOptionBlockRuleMigrator(RuleMigrationContext context)
            : base(context)
        {
            var dbTableCarrierAccount = Context.MigrationContext.DBTables[DBTableName.CarrierAccount];
            _allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;

            var dtTableSupplierZones = Context.MigrationContext.DBTables[DBTableName.SupplierZone];
            _allSupplierZones = (Dictionary<string, SupplierZone>)dtTableSupplierZones.Records;

            RouteOptionRuleManager manager = new RouteOptionRuleManager();
            _routeOptionRuleTypeId = manager.GetRuleTypeId();
        }
        public override IEnumerable<SourceRule> GetSourceRules()
        {
            List<SourceRule> routeRules = new List<SourceRule>();

            SourceRouteOptionBlockRuleDataManager dataManager = new SourceRouteOptionBlockRuleDataManager(Context.MigrationContext.ConnectionString, Context.GetEffectiveOnly);
            var blockRules = dataManager.GetRouteOptionBlockRules();

            routeRules.AddRange(GetRulesWithZone(blockRules.Where(o => o.SupplierZoneId.HasValue)));
            return routeRules;
        }
        IEnumerable<SourceRule> GetRulesWithZone(IEnumerable<SourceRouteOptionBlockRule> blockRules)
        {
            List<SourceRule> routeRules = new List<SourceRule>();
            var dicRules = GetRulesDictionary(blockRules);
            foreach (var rules in dicRules.Values)
            {
                SourceRouteOptionBlockRule sourceRule = rules.First();
                if (sourceRule == null)
                    continue;
                routeRules.Add(GetSourceRuleFromZones(rules));
            }
            return routeRules;

        }
        Dictionary<string, List<SourceRouteOptionBlockRule>> GetRulesDictionary(IEnumerable<SourceRouteOptionBlockRule> blockedRules)
        {
            Dictionary<string, List<SourceRouteOptionBlockRule>> dicRules = new Dictionary<string, List<SourceRouteOptionBlockRule>>();
            foreach (var routeRule in blockedRules)
            {
                string key = string.Format("{0}_{1}_{2}", routeRule.SupplierId, routeRule.BED, routeRule.EED);

                List<SourceRouteOptionBlockRule> lstRules;
                if (!dicRules.TryGetValue(key, out lstRules))
                {
                    lstRules = new List<SourceRouteOptionBlockRule>();
                    dicRules.Add(key, lstRules);
                }
                lstRules.Add(routeRule);
            }
            return dicRules;
        }
        SourceRule GetSourceRuleFromZones(IEnumerable<SourceRouteOptionBlockRule> rules)
        {

            SourceRouteOptionBlockRule sourceRule = rules.First();

            List<long> lstZoneIds = new List<long>();

            foreach (var rule in rules)
                if (!_allSupplierZones.ContainsKey(rule.SupplierZoneId.ToString()))
                    this.TotalRowsFailed++;
                else
                    lstZoneIds.Add(_allSupplierZones[rule.SupplierZoneId.ToString()].SupplierZoneId);

            var settings = GetRouteOptionRuleSettings(rules, sourceRule, lstZoneIds);

            return new SourceRule
            {
                Rule = new Rule
                {
                    BED = sourceRule.BED,
                    EED = sourceRule.EED,
                    TypeId = _routeOptionRuleTypeId,
                    RuleDetails = Serializer.Serialize(settings)
                }
            };
        }

        RouteOptionRule GetRouteOptionRuleSettings(IEnumerable<SourceRouteOptionBlockRule> rules, SourceRouteOptionBlockRule sourceRule,
           List<long> lstZoneIds)
        {
            RouteOptionRule settings = new RouteOptionRule()
            {
                BeginEffectiveTime = sourceRule.BED,
                EndEffectiveTime = sourceRule.EED,
                Description = sourceRule.Reason,
                Name = string.IsNullOrEmpty(sourceRule.Reason) ? "Migrated Rule" : sourceRule.Reason,
                Settings = new BlockRouteOptionRule
                {
                },
                Criteria = new RouteOptionRuleCriteria
                {
                    SuppliersWithZonesGroupSettings = new SelectiveSuppliersWithZonesGroup
                    {
                        SuppliersWithZones =
                            new List<SupplierWithZones>
                            {
                                new SupplierWithZones
                                {
                                    SupplierId = _allCarrierAccounts[sourceRule.SupplierId].CarrierAccountId,
                                    SupplierZoneIds = lstZoneIds
                                }
                            }
                    }
                }
            };
            return settings;
        }
    }
}
