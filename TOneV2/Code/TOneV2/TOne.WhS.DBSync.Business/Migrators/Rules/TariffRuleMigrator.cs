using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.BusinessEntity.MainExtensions.CustomerGroups;
using TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups;
using TOne.WhS.BusinessEntity.MainExtensions.SellingNumberPlan;
using TOne.WhS.BusinessEntity.MainExtensions.SupplierZoneGroups;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.SourceDataManger;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common;
using Vanrise.GenericData.Entities;
using Vanrise.GenericData.MainExtensions.GenericRuleCriteriaFieldValues;
using Vanrise.GenericData.Pricing;
using Vanrise.Rules.Entities;
using Vanrise.Rules.Pricing;
using Vanrise.Rules.Pricing.MainExtensions.ExtraCharge;
using Vanrise.Rules.Pricing.MainExtensions.Tariff;

namespace TOne.WhS.DBSync.Business.Migrators
{
    public class TariffRuleMigrator : RuleBaseMigrator
    {
        public override string EntityName
        {
            get { return "Tariff"; }
        }
        readonly Dictionary<string, CarrierAccount> _allCarrierAccounts;
        readonly Dictionary<string, CarrierProfile> _allCarrierProfiles;
        readonly Dictionary<string, SupplierZone> _allSupplierZones;
        readonly Dictionary<string, SaleZone> _allSaleZones;
        readonly int _ruleTypeId;
        Guid _CostDefinitionId = new Guid("5AEB0DAD-4BB8-44B4-ACBE-C8C917E88B58");
        Guid _SaleDefinitionId = new Guid("F24CB510-0B65-48C8-A723-1F6EBFEEA9E8");

        public TariffRuleMigrator(RuleMigrationContext context)
            : base(context)
        {

            var dbTableCarrierAccount = Context.MigrationContext.DBTables[DBTableName.CarrierAccount];
            _allCarrierAccounts = (Dictionary<string, CarrierAccount>)dbTableCarrierAccount.Records;


            var dbTableCarrierProfile = Context.MigrationContext.DBTables[DBTableName.CarrierProfile];
            _allCarrierProfiles = (Dictionary<string, CarrierProfile>)dbTableCarrierProfile.Records;

            var dtTableSupplierZones = Context.MigrationContext.DBTables[DBTableName.SupplierZone];
            _allSupplierZones = (Dictionary<string, SupplierZone>)dtTableSupplierZones.Records;

            var dbTableSaleZones = Context.MigrationContext.DBTables[DBTableName.SaleZone];
            _allSaleZones = (Dictionary<string, SaleZone>)dbTableSaleZones.Records;

            TariffRuleManager manager = new TariffRuleManager();
            _ruleTypeId = manager.GetRuleTypeId();


        }
        public override IEnumerable<SourceRule> GetSourceRules()
        {
            List<SourceRule> sourceRules = new List<SourceRule>();
            sourceRules.Add(GetDefaultSaleTariffRule());
            sourceRules.Add(GetDefaultSupplierTariffRule());

            SourceTariffRuleDataManager dataManager = new SourceTariffRuleDataManager(Context.MigrationContext.ConnectionString, Context.MigrationContext.EffectiveAfterDate, Context.MigrationContext.OnlyEffective);
            var tariffRules = dataManager.GetTariffRules();

            sourceRules.AddRange(GetTariffRules(tariffRules.Where(t => t.SupplierId == "SYS"), RuleType.Sale));
            sourceRules.AddRange(GetTariffRules(tariffRules.Where(t => t.SupplierId != "SYS"), RuleType.Purchase));

            return sourceRules;
        }

        #region Private Methods
        private List<SourceRule> GetTariffRules(IEnumerable<SourceTariffRule> sourceTariffRules, RuleType type)
        {
            List<SourceRule> routeRules = new List<SourceRule>();
            Dictionary<string, TariffRulesByKeyWithMaxCount> tariffRulesByKeyWithMaxCounts;
            var dicRules = GetRulesDictionary(sourceTariffRules, type, out tariffRulesByKeyWithMaxCounts);
            foreach (var item in dicRules)
            {
                TariffRulesByKeyWithMaxCount tariffRulesByKeyWithMaxCount;
                string carrierId = item.Key;

                if (tariffRulesByKeyWithMaxCounts.TryGetValue(carrierId, out tariffRulesByKeyWithMaxCount))
                {
                    routeRules.Add(GetSourceRule(tariffRulesByKeyWithMaxCount.TariffRulesByKey.TariffRules, type, true));
                }

                foreach (var tariffByKey in item.Value)
                {
                    if (tariffByKey.Value == tariffRulesByKeyWithMaxCount.TariffRulesByKey)
                        continue;
                    var rule = GetSourceRule(tariffByKey.Value.TariffRules, type, true);
                    if (rule != null)
                    {
                        routeRules.Add(rule);
                    }
                }
            }
            return routeRules;
        }

        private SourceRule GetSourceRule(List<SourceTariffRule> rules, RuleType type, bool includeZonesCriteria)
        {
            CarrierAccount carrier = null;
            SourceTariffRule defaultRule = rules.FirstOrDefault();
            TariffRule tariffRule = GetBasicTariffRule(defaultRule);

            switch (type)
            {
                case RuleType.Sale:
                    if (!_allCarrierAccounts.TryGetValue(defaultRule.CustomerId, out carrier))
                        throw new NullReferenceException(string.Format("customer not found. Customer Source Id {0}.", defaultRule.CustomerId));
                    tariffRule.Criteria.FieldsValues.Add("CustomerId", new BusinessEntityValues()
                    {
                        BusinessEntityGroup = new SelectiveCustomerGroup
                        {
                            CustomerIds = new List<int>() { carrier.CarrierAccountId }
                        }
                    });
                    if (includeZonesCriteria)
                        tariffRule.Criteria.FieldsValues.Add("SaleZoneId", new BusinessEntityValues()
                        {
                            BusinessEntityGroup = new SelectiveSaleZoneGroup
                            {
                                SellingNumberPlanId = Context.MigrationContext.DefaultSellingNumberPlanId,
                                ZoneIds = GetZoneIds(rules, type).ToList()
                            }
                        });

                    tariffRule.Settings.CurrencyId = carrier.CarrierAccountSettings == null ? Context.CurrencyId : carrier.CarrierAccountSettings.CurrencyId;
                    tariffRule.Description = string.Format("Migrated Sale Tariff Rule {0}", Context.Counter++);
                    tariffRule.DefinitionId = _SaleDefinitionId;
                    break;
                case RuleType.Purchase:
                    if (!_allCarrierAccounts.TryGetValue(defaultRule.SupplierId, out carrier))
                        throw new NullReferenceException(string.Format("Supplier not found. Supplier Source Id {0}.", defaultRule.CustomerId));
                    CarrierProfile profile;
                    if (!_allCarrierProfiles.TryGetValue(defaultRule.SupplierProfileID, out profile))
                        throw new NullReferenceException(string.Format("Profile not found. Profile Source Id {0}.", defaultRule.SupplierProfileID));

                    profile.ThrowIfNull("profile.Settings", defaultRule.SupplierProfileID);
                    tariffRule.Settings.CurrencyId = profile.Settings.CurrencyId;
                    tariffRule.Description = string.Format("Migrated Supplier Tariff Rule {0}", Context.Counter++);
                    tariffRule.DefinitionId = _CostDefinitionId;

                    if (includeZonesCriteria)
                    {
                        tariffRule.Criteria.FieldsValues.Add("SupplierZoneId", new BusinessEntityValues()
                        {
                            BusinessEntityGroup = new SelectiveSupplierZoneGroup()
                            {
                                SuppliersWithZones = new List<SupplierWithZones>()
                            {
                               new SupplierWithZones
                                {
                                    SupplierId = carrier.CarrierAccountId,
                                    SupplierZoneIds =   GetZoneIds(rules, type).ToList()
                                }
                            }
                            }
                        });
                    }
                    else
                    {
                        tariffRule.Criteria.FieldsValues.Add("SupplierId", new StaticValues
                        {
                            Values = ((new List<int> { carrier.CarrierAccountId }).Cast<Object>()).ToList()
                        });
                    }
                    break;
            }
            SourceRule sourceRule = new SourceRule
            {
                Rule = new Rule
                {
                    BED = RuleMigrator.s_defaultRuleBED,
                    EED = null,
                    TypeId = _ruleTypeId,
                    RuleDetails = Serializer.Serialize(tariffRule)
                }
            };

            return sourceRule;
        }

        private static TariffRule GetBasicTariffRule(SourceTariffRule defaultRule)
        {
            TariffRule tariffRule = new TariffRule
            {
                BeginEffectiveTime = defaultRule.BED,
                EndEffectiveTime = defaultRule.EED,

                Settings = new Vanrise.Rules.Pricing.MainExtensions.Tariff.RegularTariffSettings
                {
                    FractionUnit = defaultRule.FractionUnit,
                    FirstPeriod = defaultRule.FirstPeriod,
                    CallFee = defaultRule.CallFee,
                    FirstPeriodRate = defaultRule.FirstPeriodRate,
                    PricingUnit = 60,
                    FirstPeriodRateType = FirstPeriodRateType.EffectiveRate
                },

                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>()
                }
            };
            return tariffRule;
        }
        HashSet<long> GetZoneIds(List<SourceTariffRule> rules, RuleType type)
        {
            HashSet<long> zoneIds = new HashSet<long>();
            switch (type)
            {
                case RuleType.Sale:
                    foreach (SourceTariffRule sourcetariff in rules)
                    {
                        if (!_allSaleZones.ContainsKey(sourcetariff.ZoneId.ToString()))
                        {
                            Context.MigrationContext.WriteWarning(string.Format("Failed migrating Tariff Rule, Source Id: {0}, Sale Zone Id {1}", sourcetariff.SourceId, sourcetariff.ZoneId));
                            this.TotalRowsFailed++;
                        }
                        else
                            zoneIds.Add(_allSaleZones[sourcetariff.ZoneId.ToString()].SaleZoneId);
                    }
                    break;
                case RuleType.Purchase:
                    foreach (SourceTariffRule sourcetariff in rules)
                    {
                        if (!_allSupplierZones.ContainsKey(sourcetariff.ZoneId.ToString()))
                        {
                            Context.MigrationContext.WriteWarning(string.Format("Failed migrating Tariff Rule, Source Id: {0}, Supplier Zone Id {1}", sourcetariff.SourceId, sourcetariff.ZoneId));
                            this.TotalRowsFailed++;
                        }
                        else
                            zoneIds.Add(_allSupplierZones[sourcetariff.ZoneId.ToString()].SupplierZoneId);
                    }
                    break;

            }
            return zoneIds;
        }
        Dictionary<string, Dictionary<string, TariffRulesByKey>> GetRulesDictionary(IEnumerable<SourceTariffRule> tariffRules, RuleType type, out Dictionary<string, TariffRulesByKeyWithMaxCount> tariffRulesByKeyWithMaxCounts)
        {
            tariffRulesByKeyWithMaxCounts = new Dictionary<string, TariffRulesByKeyWithMaxCount>();

            Dictionary<string, Dictionary<string, TariffRulesByKey>> dicRules = new Dictionary<string, Dictionary<string, TariffRulesByKey>>();
            foreach (var tariffRule in tariffRules)
            {
                string carrierId = type == RuleType.Sale ? tariffRule.CustomerId : tariffRule.SupplierId;
                TariffRulesByKeyWithMaxCount tariffRulesByKeyWithMaxCount = tariffRulesByKeyWithMaxCounts.GetOrCreateItem(carrierId);

                Dictionary<string, TariffRulesByKey> dictariffRules = dicRules.GetOrCreateItem(carrierId);

                string key = GetTariffRuleKey(tariffRule, type);

                TariffRulesByKey lstRules;
                if (!dictariffRules.TryGetValue(key, out lstRules))
                {
                    lstRules = new TariffRulesByKey();
                    dictariffRules.Add(key, lstRules);
                }
                lstRules.TariffRules.Add(tariffRule);
                lstRules.Counter++;
                if (tariffRulesByKeyWithMaxCount.MaxCount < lstRules.Counter)
                {
                    tariffRulesByKeyWithMaxCount.MaxCount = lstRules.Counter;
                    tariffRulesByKeyWithMaxCount.TariffRulesByKey = lstRules;
                }
            }


            return dicRules;
        }
        SourceRule GetDefaultSupplierTariffRule()
        {
            TariffRule supplierTariffRule = new TariffRule
            {
                BeginEffectiveTime = RuleMigrator.s_defaultRuleBED,
                EndEffectiveTime = null,
                Settings = new Vanrise.Rules.Pricing.MainExtensions.Tariff.RegularTariffSettings
                {
                    PricingUnit = 60,
                    FractionUnit = 1,
                    CurrencyId = Context.CurrencyId,
                    FirstPeriodRateType = Vanrise.Rules.Pricing.MainExtensions.Tariff.FirstPeriodRateType.EffectiveRate
                },
                DefinitionId = _CostDefinitionId,
                Description = "Default Supplier Tariff Rule"
            };
            SourceRule defaultRouteRule = new SourceRule
            {
                Rule = new Rule
                {
                    BED = RuleMigrator.s_defaultRuleBED,
                    EED = null,
                    TypeId = _ruleTypeId,
                    RuleDetails = Serializer.Serialize(supplierTariffRule)
                }
            };

            return defaultRouteRule;
        }
        SourceRule GetDefaultSaleTariffRule()
        {
            TariffRule saleTariffRule = new TariffRule
            {
                BeginEffectiveTime = RuleMigrator.s_defaultRuleBED,
                EndEffectiveTime = null,
                Settings = new Vanrise.Rules.Pricing.MainExtensions.Tariff.RegularTariffSettings
                {
                    PricingUnit = 60,
                    FractionUnit = 1,
                    CurrencyId = Context.CurrencyId,
                    FirstPeriodRateType = Vanrise.Rules.Pricing.MainExtensions.Tariff.FirstPeriodRateType.EffectiveRate
                },
                DefinitionId = _SaleDefinitionId,
                Description = "Default Sale Tariff Rule"
            };

            SourceRule defaultRouteRule = new SourceRule
            {
                Rule = new Rule
                {
                    BED = RuleMigrator.s_defaultRuleBED,
                    EED = null,
                    TypeId = _ruleTypeId,
                    RuleDetails = Serializer.Serialize(saleTariffRule)
                }
            };

            return defaultRouteRule;
        }
        string GetTariffRuleKey(SourceTariffRule rule, RuleType ruleType)
        {
            switch (ruleType)
            {
                case RuleType.Sale:
                    return string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}", rule.CustomerId, rule.BED, rule.EED, rule.FirstPeriod, rule.FirstPeriodRate, rule.FractionUnit, rule.CallFee);
                case RuleType.Purchase:
                    return string.Format("{0}-{1}-{2}-{3}-{4}-{5}-{6}", rule.SupplierId, rule.BED, rule.EED, rule.FirstPeriod, rule.FirstPeriodRate, rule.FractionUnit, rule.CallFee);
                default:
                    return "";
            }
        }

        #endregion

    }

    class TariffRulesByKey
    {
        public TariffRulesByKey()
        {
            this.TariffRules = new List<SourceTariffRule>();
        }
        public List<SourceTariffRule> TariffRules { get; set; }
        public int Counter { get; set; }
        public string CarrierId { get; set; }
    }

    class TariffRulesByKeyWithMaxCount
    {
        public TariffRulesByKey TariffRulesByKey { get; set; }
        public int MaxCount { get; set; }
    }
}
