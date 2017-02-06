﻿using System;
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

            SourceTariffRuleDataManager dataManager = new SourceTariffRuleDataManager(Context.MigrationContext.ConnectionString, true);
            var tariffRules = dataManager.GetTariffRules();

            sourceRules.AddRange(GetTariffRules(tariffRules.Where(t => t.SupplierId == "SYS"), RuleType.Sale));
            sourceRules.AddRange(GetTariffRules(tariffRules.Where(t => t.SupplierId != "SYS"), RuleType.Purchase));

            return sourceRules;
        }

        #region Private Methods
        private List<SourceRule> GetTariffRules(IEnumerable<SourceTariffRule> sourceTariffRules, RuleType type)
        {
            List<SourceRule> routeRules = new List<SourceRule>();
            var dicRules = GetRulesDictionary(sourceTariffRules, type);
            foreach (var rules in dicRules.Values)
            {
                SourceTariffRule sourceRule = rules.First();
                if (sourceRule == null)
                    continue;
                var rule = GetSourceRule(rules, type);
                if (rule == null)
                {
                    this.TotalRowsFailed++;
                    continue;
                }
                routeRules.Add(rule);
            }
            return routeRules;
        }
        private SourceRule GetSourceRule(List<SourceTariffRule> rules, RuleType type)
        {
            CarrierAccount carrier = null;
            SourceTariffRule defaultRule = rules.FirstOrDefault();
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
                    FirstPeriodRateType = FirstPeriodRateType.FixedRate
                },

                Criteria = new GenericRuleCriteria
                {
                    FieldsValues = new Dictionary<string, GenericRuleCriteriaFieldValues>()
                }
            };

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
                    tariffRule.Settings.CurrencyId = Context.CurrencyId;
                    tariffRule.Description = string.Format("Migrated Supplier Tariff Rule {0}", Context.Counter++);
                    tariffRule.DefinitionId = _CostDefinitionId;
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
        HashSet<long> GetZoneIds(List<SourceTariffRule> rules, RuleType type)
        {
            HashSet<long> zoneIds = new HashSet<long>();
            switch (type)
            {
                case RuleType.Sale:
                    foreach (SourceTariffRule sourcetariff in rules)
                    {
                        if (!_allSaleZones.ContainsKey(sourcetariff.ZoneId.ToString()))
                            this.TotalRowsFailed++;
                        else
                            zoneIds.Add(_allSaleZones[sourcetariff.ZoneId.ToString()].SaleZoneId);
                    }
                    break;
                case RuleType.Purchase:
                    foreach (SourceTariffRule sourcetariff in rules)
                    {
                        if (!_allSupplierZones.ContainsKey(sourcetariff.ZoneId.ToString()))
                            this.TotalRowsFailed++;
                        else
                            zoneIds.Add(_allSupplierZones[sourcetariff.ZoneId.ToString()].SupplierZoneId);
                    }
                    break;

            }
            return zoneIds;
        }
        Dictionary<string, List<SourceTariffRule>> GetRulesDictionary(IEnumerable<SourceTariffRule> tariffRules, RuleType type)
        {
            Dictionary<string, List<SourceTariffRule>> dicRules = new Dictionary<string, List<SourceTariffRule>>();
            foreach (var tariffRule in tariffRules)
            {
                string key = GetTariffRuleKey(tariffRule, type);

                List<SourceTariffRule> lstRules;
                if (!dicRules.TryGetValue(key, out lstRules))
                {
                    lstRules = new List<SourceTariffRule>();
                    dicRules.Add(key, lstRules);
                }
                lstRules.Add(tariffRule);
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
                    FractionUnit = 0,
                    CurrencyId = Context.CurrencyId,
                    FirstPeriodRateType = Vanrise.Rules.Pricing.MainExtensions.Tariff.FirstPeriodRateType.FixedRate,
                    FirstPeriodRate = 0

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
                    FractionUnit = 0,
                    CurrencyId = Context.CurrencyId,
                    FirstPeriodRateType = Vanrise.Rules.Pricing.MainExtensions.Tariff.FirstPeriodRateType.FixedRate,
                    FirstPeriodRate = 0
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
}
