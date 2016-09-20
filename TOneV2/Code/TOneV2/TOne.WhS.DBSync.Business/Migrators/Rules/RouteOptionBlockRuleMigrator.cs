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

namespace TOne.WhS.DBSync.Business
{
    public class RouteOptionBlockRuleMigrator : RouteRuleBaseMigrator
    {
        readonly Dictionary<string, CarrierAccount> _allCarrierAccounts;
        readonly Dictionary<string, SaleZone> _allSaleZones;
        readonly Dictionary<string, SupplierZone> _allSupplierZones;
        public RouteOptionBlockRuleMigrator(RuleMigrationContext context)
            : base(context)
        {
            var dbTableCarrierAccount = Context.MigrationContext.DBTables[DBTableName.CarrierAccount];
            _allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;

            var dbTableSaleZones = Context.MigrationContext.DBTables[DBTableName.SaleZone];
            _allSaleZones = (Dictionary<string, SaleZone>)dbTableSaleZones.Records;

            var dtTableSupplierZones = Context.MigrationContext.DBTables[DBTableName.SupplierZone];
            _allSupplierZones = (Dictionary<string, SupplierZone>)dtTableSupplierZones.Records;
        }
        public override IEnumerable<SourceRule> GetRouteRules()
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
                string key = routeRule.SupplierId;

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
                lstZoneIds.Add(_allSupplierZones[rule.SupplierZoneId.ToString()].SupplierZoneId);
            return new SourceRule
            {
                RouteRule = new RouteOptionRule()
                {
                    BeginEffectiveTime = rules.Min(r => r.BED),
                    EndEffectiveTime = null,
                    Description = sourceRule.Reason,
                    Name = string.IsNullOrEmpty(sourceRule.Reason) ? "Migrated Rule" : sourceRule.Reason,
                    Settings = new BlockRouteOptionRule
                    {
                    },
                    Criteria = new RouteOptionRuleCriteria
                    {
                        SuppliersWithZonesGroupSettings = new SelectiveSuppliersWithZonesGroup
                        {
                            SuppliersWithZones = new List<SupplierWithZones> { new SupplierWithZones { SupplierId = _allCarrierAccounts[sourceRule.SupplierId].CarrierAccountId, SupplierZoneIds = lstZoneIds } }
                             ,
                        }
                    }
                }
            };
        }
    }
}
