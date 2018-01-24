using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.MainExtensions.CodeCriteriaGroups;
using TOne.WhS.BusinessEntity.MainExtensions.CustomerGroups;
using TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups;
using TOne.WhS.DBSync.Data.SQL.SourceDataManger;
using TOne.WhS.DBSync.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Business.RouteRules.OptionSettingsGroups;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Rules.Entities;

namespace TOne.WhS.DBSync.Business
{
    class RouteBlockRuleMigrator : RuleBaseMigrator
    {
        readonly Dictionary<string, CarrierAccount> _allCarrierAccounts;
        readonly Dictionary<string, SaleZone> _allSaleZones;
        readonly int _routeRuleTypeId;
        public override string EntityName
        {
            get { return "Route Block"; }
        }
        public RouteBlockRuleMigrator(RuleMigrationContext context)
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

            SourceRouteBlockRuleDataManager dataManager = new SourceRouteBlockRuleDataManager(Context.MigrationContext.ConnectionString, Context.MigrationContext.EffectiveAfterDate, Context.MigrationContext.OnlyEffective);
            var overrideRules = dataManager.GetRouteblockRules();

            routeRules.AddRange(GetRulesWithCode(overrideRules.Where(o => !o.SaleZoneId.HasValue)));
            routeRules.AddRange(GetRulesWithZone(overrideRules.Where(o => o.SaleZoneId.HasValue)));

            return routeRules;
        }

        Dictionary<string, List<SourceRouteOverrideRule>> GetRulesDictionary(IEnumerable<SourceRouteOverrideRule> sourceRules)
        {
            Dictionary<string, List<SourceRouteOverrideRule>> dicRules = new Dictionary<string, List<SourceRouteOverrideRule>>();
            foreach (var routeRule in sourceRules)
            {
                string key = string.Format("{0},{1},{2},{3},{4}", routeRule.CustomerId, routeRule.Code, routeRule.ExcludedCodes, routeRule.BED, routeRule.EED);

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

        #region Zone Part
        IEnumerable<SourceRule> GetRulesWithZone(IEnumerable<SourceRouteOverrideRule> sourceRules)
        {
            List<SourceRule> routeRules = new List<SourceRule>();
            var dicRules = GetRulesDictionary(sourceRules);
            foreach (var rules in dicRules.Values)
            {
                SourceRouteOverrideRule sourceRule = rules.First();
                if (sourceRule == null)
                    continue;
                var rule = GetSourceRuleFromZones(rules);
                if (rule != null)
                {
                    routeRules.Add(rule);
                }
            }
            return routeRules;
        }
        SourceRule GetSourceRuleFromZones(IEnumerable<SourceRouteOverrideRule> rules)
        {
            SourceRouteOverrideRule sourceRule = rules.First();

            List<long> lstZoneIds = new List<long>();

            foreach (var rule in rules)
                if (!_allSaleZones.ContainsKey(rule.SaleZoneId.ToString()))
                {
                    Context.MigrationContext.WriteWarning(string.Format("Failed migrating Route Block, Source Id: {0}, Sale Zone Id {1} does not exist", rule.SourceId, rule.SaleZoneId));
                    this.TotalRowsFailed++;
                }
                else
                    lstZoneIds.Add(_allSaleZones[rule.SaleZoneId.ToString()].SaleZoneId);

            var ruleDetails = GetRuleDetailsFromZone(rules, sourceRule, lstZoneIds);
            if (ruleDetails == null || lstZoneIds.Count == 0)
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
        RouteRule GetRuleDetailsFromZone(IEnumerable<SourceRouteOverrideRule> rules, SourceRouteOverrideRule sourceRule, List<long> lstZoneIds)
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
                    Name = string.Format("Migrated Route Block Rule {0}", Context.Counter++),
                    Criteria = criteria,
                    Settings = new BlockRouteRule()
                };
                return details;
            }
        }
        RouteRuleCriteria GetRuleZoneCriteria(List<long> lstZoneIds, SourceRouteOverrideRule sourceRule)
        {
            CarrierAccount customer;
            if (!_allCarrierAccounts.TryGetValue(sourceRule.CustomerId, out customer))
            {
                Context.MigrationContext.WriteWarning(string.Format("Failed migrating Route Block, Source Id: {0}, Customer Id {1} does not exist", sourceRule.SourceId, sourceRule.CustomerId));
                TotalRowsFailed++;
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

        #endregion

        #region Code Part
        IEnumerable<SourceRule> GetRulesWithCode(IEnumerable<SourceRouteOverrideRule> sourceRules)
        {
            List<SourceRule> routeRules = new List<SourceRule>();
            var dicRules = GetRulesDictionary(sourceRules);
            foreach (var rules in dicRules.Values)
            {
                SourceRouteOverrideRule sourceRule = rules.First();
                if (sourceRule == null)
                    continue;
                var rule = GetSourceRuleFromCodes(rules);
                if (rule != null)
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
                    Name = string.Format("Migrated Route Block Rule {0}", Context.Counter++),
                    Criteria = criteria,
                    Settings = new BlockRouteRule()
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
                Context.MigrationContext.WriteWarning(string.Format("Failed migrating Route Block, Source Id: {0}, Customer Id {1} does not exist", sourceRule.SourceId, sourceRule.CustomerId));
                TotalRowsFailed++;
                return null;
            }
            else
            {
                RouteRuleCriteria routeRuleCriteria = new RouteRuleCriteria
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

                if (sourceRule.ExcludedCodesList != null)
                    routeRuleCriteria.ExcludedDestinations = new ExcludedCodes() { Codes = sourceRule.ExcludedCodesList.ToList() };

                return routeRuleCriteria;
            }
        }

        #endregion

    }

}
