using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.MainExtensions.CodeCriteriaGroups;
using TOne.WhS.BusinessEntity.MainExtensions.CustomerGroups;
using TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups;
using TOne.WhS.BusinessEntity.MainExtensions.SuppliersWithZonesGroups;
using TOne.WhS.DBSync.Data.SQL.SourceDataManger;
using TOne.WhS.DBSync.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Rules.Entities;

namespace TOne.WhS.DBSync.Business.Migrators
{
    public class RouteOptionBlockFromOverrideRuleMigrator : RuleBaseMigrator
    {
        public override string EntityName
        {
            get { return "Route Option Block From Override"; }
        }

        readonly Dictionary<string, SaleZone> _allSaleZones;
        readonly Dictionary<string, CarrierAccount> _allCarrierAccounts;
        readonly Dictionary<string, SupplierZone> _allSupplierZones;
        readonly int _routeOptionRuleTypeId;
        public RouteOptionBlockFromOverrideRuleMigrator(RuleMigrationContext context)
            : base(context)
        {

            var dbTableCarrierAccount = Context.MigrationContext.DBTables[DBTableName.CarrierAccount];
            _allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;

            var dtTableSupplierZones = Context.MigrationContext.DBTables[DBTableName.SupplierZone];
            _allSupplierZones = (Dictionary<string, SupplierZone>)dtTableSupplierZones.Records;

            var dbTableSaleZones = Context.MigrationContext.DBTables[DBTableName.SaleZone];
            _allSaleZones = (Dictionary<string, SaleZone>)dbTableSaleZones.Records;

            RouteOptionRuleManager manager = new RouteOptionRuleManager();
            _routeOptionRuleTypeId = manager.GetRuleTypeId();
        }
        public override IEnumerable<SourceRule> GetSourceRules()
        {
            List<SourceRule> routeRules = new List<SourceRule>();

            SourceRouteOverrideRuleDataManager dataManager = new SourceRouteOverrideRuleDataManager(Context.MigrationContext.ConnectionString, Context.MigrationContext.EffectiveAfterDate, Context.MigrationContext.OnlyEffective);
            var blockRules = dataManager.GetRouteOverrideOptionBlockRules();

            routeRules.AddRange(GetRules(blockRules));
            return routeRules;
        }

        IEnumerable<SourceRule> GetRules(IEnumerable<SourceRouteOverrideRule> blockRules)
        {
            List<SourceRule> routeRules = new List<SourceRule>();
            var dicRules = GetBlockRulesDictionary(blockRules);
            foreach (var rules in dicRules.Values)
            {
                SourceRouteOverrideRule sourceRule = rules.First();
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
        private Dictionary<string, List<SourceRouteOverrideRule>> GetBlockRulesDictionary(IEnumerable<SourceRouteOverrideRule> blockedRules)
        {
            Dictionary<string, List<SourceRouteOverrideRule>> dicRules = new Dictionary<string, List<SourceRouteOverrideRule>>();
            foreach (var routeRule in blockedRules)
            {
                string key = string.Format("{0},{1},{2},{3},{4},{5}", routeRule.CustomerId,
                    routeRule.BlockedOptionsString, routeRule.Code, routeRule.ExcludedCodes, routeRule.BED, routeRule.EED);

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

        SourceRule BuildSourceRule(IEnumerable<SourceRouteOverrideRule> rules)
        {
            SourceRouteOverrideRule sourceRule = rules.First();

            var settings = GetRouteOptionRuleSettings(rules, sourceRule);
            if (settings == null)
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
        RouteOptionRule GetRouteOptionRuleSettings(IEnumerable<SourceRouteOverrideRule> rules, SourceRouteOverrideRule sourceRule)
        {
            List<int> lstZoneIds = rules.Where(r => r.SaleZoneId.HasValue).Select(r => r.SaleZoneId.Value).ToList();
            RouteOptionRule settings = new RouteOptionRule()
            {
                BeginEffectiveTime = sourceRule.BED,
                EndEffectiveTime = sourceRule.EED,
                Description = sourceRule.Reason,
                Name = string.Format("Migrated Route Option Block Rule From Route Override {0}", Context.Counter++),
                Settings = new BlockRouteOptionRule
                {
                },
                Criteria = new RouteOptionRuleCriteria
                {
                    SuppliersWithZonesGroupSettings = new SelectiveSuppliersWithZonesGroup
                    {
                        SuppliersWithZones = GetBlockedOptions(sourceRule.BlockedOptions)

                    }
                }
            };

            if (!string.IsNullOrEmpty(sourceRule.CustomerId))
            {
                CarrierAccount customer;
                if (!_allCarrierAccounts.TryGetValue(sourceRule.CustomerId, out customer))
                {
                    Context.MigrationContext.WriteWarning(string.Format("Failed migrating Route Override, Source Id: {0}, Customer Id {1}", sourceRule.SourceId, sourceRule.CustomerId));
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
                if (sourceRule.ExcludedCodesList != null)
                    settings.Criteria.ExcludedDestinations = new ExcludedCodes() { Codes = sourceRule.ExcludedCodesList.ToList() };
            }
            if (lstZoneIds.Count > 0)
            {
                List<long> zoneIds = new List<long>();
                foreach (var zoneId in lstZoneIds)
                {
                    if (!_allSaleZones.ContainsKey(zoneId.ToString()))
                    {
                        Context.MigrationContext.WriteWarning(string.Format("Failed migrating Route Override, Source Id: {0}, Sale Zone Id {1}", sourceRule.SourceId, zoneId));
                        this.TotalRowsFailed++;
                    }
                    else
                        zoneIds.Add(_allSaleZones[zoneId.ToString()].SaleZoneId);
                }
                if (zoneIds.Count == 0)
                    return null;
                settings.Criteria.SaleZoneGroupSettings = new SelectiveSaleZoneGroup { ZoneIds = zoneIds, SellingNumberPlanId = Context.MigrationContext.DefaultSellingNumberPlanId };

            }
            return settings;
        }

        private List<SupplierWithZones> GetBlockedOptions(IEnumerable<BlockedOption> blockedOptions)
        {
            List<SupplierWithZones> result = new List<SupplierWithZones>();
            foreach (var bOption in blockedOptions)
            {
                CarrierAccount supplier;
                if (!_allCarrierAccounts.TryGetValue(bOption.SupplierId, out supplier))
                    continue;
                result.Add(new SupplierWithZones { SupplierId = supplier.CarrierAccountId });
            }
            return result;
        }
    }
}
