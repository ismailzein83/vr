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

            SourceRouteOptionBlockRuleDataManager dataManager = new SourceRouteOptionBlockRuleDataManager(Context.MigrationContext.ConnectionString, Context.MigrationContext.EffectiveAfterDate, Context.MigrationContext.OnlyEffective);
            var blockRules = dataManager.GetRouteOptionBlockRules();

            routeRules.AddRange(GetRules(blockRules.Where(o => o.SupplierZoneId.HasValue)));
            routeRules.AddRange(GetRules(blockRules.Where(o => !o.SupplierZoneId.HasValue)));
            return routeRules;
        }
        IEnumerable<SourceRule> GetRules(IEnumerable<SourceRouteOptionBlockRule> blockRules)
        {
            List<SourceRule> routeRules = new List<SourceRule>();
            var dicRules = GetRulesDictionary(blockRules);
            foreach (var rules in dicRules.Values)
            {
                SourceRouteOptionBlockRule sourceRule = rules.First();
                if (sourceRule == null)
                    continue;
                SourceRule rule = BuildSourceRule(rules);
                if (rule != null)
                {
                    routeRules.Add(rule);
                }

            }
            return routeRules;

        }
        Dictionary<string, List<SourceRouteOptionBlockRule>> GetRulesDictionary(IEnumerable<SourceRouteOptionBlockRule> blockedRules)
        {
            Dictionary<string, List<SourceRouteOptionBlockRule>> dicRules = new Dictionary<string, List<SourceRouteOptionBlockRule>>();
            foreach (var routeRule in blockedRules)
            {
                string key = string.Format("{0}_{1}_{2}_{3}_{4}_{5}", routeRule.SupplierId, routeRule.CustomerId, routeRule.Code, routeRule.ExcludedCodes, routeRule.BED, routeRule.EED);

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
        SourceRule BuildSourceRule(IEnumerable<SourceRouteOptionBlockRule> rules)
        {
            List<long> lstZoneIds = new List<long>();
            SourceRouteOptionBlockRule sourceRule = rules.First();
            if (sourceRule.SupplierZoneId.HasValue)
            {
                foreach (var rule in rules)
                    if (!_allSupplierZones.ContainsKey(rule.SupplierZoneId.ToString()))
                    {
                        Context.MigrationContext.WriteWarning(string.Format("Failed migrating Route Option Block, Source Id: {0}, Source Supplier Zone Id {1}, First Rule Source Supplier Zone Id {2}", rule.SourceId, rule.SupplierZoneId, sourceRule.SupplierZoneId));
                        this.TotalRowsFailed++;
                    }
                    else
                        lstZoneIds.Add(_allSupplierZones[rule.SupplierZoneId.ToString()].SupplierZoneId);
            }
            var settings = GetRouteOptionRuleSettings(rules, sourceRule, lstZoneIds);
            if (settings == null || (lstZoneIds.Count == 0 && sourceRule.SupplierZoneId.HasValue))
                return null;

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
            if (lstZoneIds == null && string.IsNullOrEmpty(sourceRule.Code))
                return null;

            RouteOptionRule settings = new RouteOptionRule()
            {
                BeginEffectiveTime = sourceRule.BED,
                EndEffectiveTime = sourceRule.EED,
                Description = sourceRule.Reason,
                Name = string.Format("Migrated Route Option Block Rule {0}", Context.Counter++),
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

            if (!string.IsNullOrEmpty(sourceRule.CustomerId))
            {
                CarrierAccount customer;
                if (!_allCarrierAccounts.TryGetValue(sourceRule.CustomerId, out customer))
                {
                    Context.MigrationContext.WriteWarning(string.Format("Failed migrating Route Option Block, Source Id: {0}, Customer Id {1}", sourceRule.SourceId, sourceRule.CustomerId));
                    TotalRowsFailed++;
                    return null;
                }
                settings.Criteria.CustomerGroupSettings = new SelectiveCustomerGroup
                        {
                            CustomerIds = new List<int>() { customer.CarrierAccountId },
                        };
            }

            if (!string.IsNullOrEmpty(sourceRule.Code))
            {
                settings.Criteria.CodeCriteriaGroupSettings = new SelectiveCodeCriteriaGroup
                {
                    Codes = new List<CodeCriteria>() { 
                        new CodeCriteria
                        {
                            Code = sourceRule.Code, WithSubCodes = sourceRule.IncludeSubCode
                        } 
                    },
                };
                if (sourceRule.ExcludedCodesList != null && sourceRule.ExcludedCodesList.Count > 0)
                    settings.Criteria.ExcludedDestinations = new ExcludedCodes() { Codes = sourceRule.ExcludedCodesList.ToList() };
            }
            return settings;
        }
    }
}
