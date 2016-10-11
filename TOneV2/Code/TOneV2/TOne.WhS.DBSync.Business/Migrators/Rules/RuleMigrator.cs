using System;
using System.Collections.Generic;
using TOne.WhS.DBSync.Business.Migrators;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using TOne.WhS.Routing.Business;
using TOne.WhS.Routing.Business.RouteRules.Filters;
using TOne.WhS.Routing.Business.RouteRules.Orders;
using TOne.WhS.Routing.Entities;
using Vanrise.Common;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.GenericData.Pricing;
using Vanrise.Rules;
using Vanrise.Rules.Entities;
using Vanrise.Rules.Pricing;

namespace TOne.WhS.DBSync.Business
{
    public class RuleMigrator : Migrator<SourceRule, Rule>
    {
        readonly RulesDBSyncDataManager _dbSyncDataManager;
        RuleBaseMigrator _routeRuleBaseMigrator;
        CurrencySettingData _currencySettingData;
        public RuleMigrator(MigrationContext context)
            : base(context)
        {

            _dbSyncDataManager = new RulesDBSyncDataManager(context.UseTempTables);

            SettingManager settingManager = new SettingManager();
            var _systemCurrencySetting = settingManager.GetSettingByType("VR_Common_BaseCurrency");
            _currencySettingData = (CurrencySettingData)_systemCurrencySetting.Data;
        }
        public override void FillTableInfo(bool useTempTables)
        {

        }
        public override IEnumerable<SourceRule> GetSourceItems()
        {
            List<SourceRule> routeRules = new List<SourceRule>();
            routeRules.AddRange(GetDefaultRules());

            RuleMigrationContext ruleContext = new RuleMigrationContext
            {
                MigrationContext = new MigrationContext
                {
                    ConnectionString = Context.ConnectionString,
                    DBTables = Context.DBTables,
                    UseTempTables = Context.UseTempTables,
                    DefaultSellingNumberPlanId = Context.DefaultSellingNumberPlanId

                },
                CurrencyId = _currencySettingData.CurrencyId

            };

            foreach (RuleEntitiesEnum ruleEntitiesEnum in Enum.GetValues(typeof(RuleEntitiesEnum)))
            {
                switch (ruleEntitiesEnum)
                {
                    case RuleEntitiesEnum.RouteOverride:
                        _routeRuleBaseMigrator = new RouteOverrideRuleMigrator(ruleContext);
                        break;
                    case RuleEntitiesEnum.RouteOptionBlock:
                        _routeRuleBaseMigrator = new RouteOptionBlockRuleMigrator(ruleContext);
                        break;
                    case RuleEntitiesEnum.Commission:
                        _routeRuleBaseMigrator = new CommissionRuleMigrator(ruleContext);
                        break;
                    case RuleEntitiesEnum.SaleMarketPrice:
                        _routeRuleBaseMigrator = new MarketPriceRuleMigrator(ruleContext);
                        break;
                    default:
                        _routeRuleBaseMigrator = null;
                        break;
                }
                if (_routeRuleBaseMigrator != null)
                {
                    routeRules.AddRange(_routeRuleBaseMigrator.GetSourceRules());
                    _routeRuleBaseMigrator.WriteFaildRowsLog();
                }
            }

            return routeRules;
        }
        IEnumerable<SourceRule> GetDefaultRules()
        {
            List<SourceRule> defaultRules = new List<SourceRule>();

            RouteRuleManager manager = new RouteRuleManager();

            RouteRule rule = new RouteRule
            {
                Criteria = new RouteRuleCriteria
                {
                },
                Settings = new RegularRouteRule
                {
                    OptionOrderSettings = new List<RouteOptionOrderSettings>
                    {
                        new OptionOrderByRate()
                    },
                    OptionFilters = new List<RouteOptionFilterSettings> 
                    {
                        new ServiceOptionFilter(),
                        new RateOptionFilter
                        {
                    RateOption = RateOption.MaximumLoss,
                    RateOptionType = RateOptionType.Fixed,
                    RateOptionValue = 0
                    }
                    }
                },
                BeginEffectiveTime = DateTime.Now,
                Description = "Default Routing Rule",
                Name = "Default Routing Rule"
            };

            defaultRules.Add(GetSourceRule(manager.GetRuleTypeId(), Serializer.Serialize(rule)));

            TariffRuleManager tariffManager = new TariffRuleManager();

            TariffRule saleTariffRule = new TariffRule
            {
                BeginEffectiveTime = DateTime.Now,
                EndEffectiveTime = null,
                Settings = new Vanrise.Rules.Pricing.MainExtensions.Tariff.RegularTariffSettings
                {
                    CallFee = 0.5M,
                    FirstPeriod = 60,
                    FirstPeriodRate = 0.5M,
                    FractionUnit = 60,
                    PricingUnit = 60,
                    CurrencyId = _currencySettingData.CurrencyId
                },
                DefinitionId = new Guid("F24CB510-0B65-48C8-A723-1F6EBFEEA9E8"),
                Description = "Default Sale Tariff Rule"
            };


            defaultRules.Add(GetSourceRule(tariffManager.GetRuleTypeId(), Serializer.Serialize(saleTariffRule)));

            TariffRule supplierTariffRule = new TariffRule
            {
                BeginEffectiveTime = DateTime.Now,
                EndEffectiveTime = null,
                Settings = new Vanrise.Rules.Pricing.MainExtensions.Tariff.RegularTariffSettings
                {
                    CallFee = 0.5M,
                    FirstPeriod = 60,
                    FirstPeriodRate = 0.5M,
                    FractionUnit = 60,
                    PricingUnit = 60,
                    CurrencyId = _currencySettingData.CurrencyId
                },
                DefinitionId = new Guid("5AEB0DAD-4BB8-44B4-ACBE-C8C917E88B58"),
                Description = "Default Supplier Tariff Rule"
            };

            defaultRules.Add(GetSourceRule(tariffManager.GetRuleTypeId(), Serializer.Serialize(supplierTariffRule)));

            return defaultRules;
        }
        SourceRule GetSourceRule(int ruleTypeId, string ruleSerialized)
        {
            SourceRule defaultRouteRule = new SourceRule
            {
                Rule = new Rule
                {
                    BED = DateTime.Now,
                    EED = null,
                    TypeId = ruleTypeId,
                    RuleDetails = ruleSerialized
                }
            };
            return defaultRouteRule;
        }
        public override Rule BuildItemFromSource(SourceRule sourceItem)
        {
            return sourceItem.Rule;
        }
        public override void AddItems(List<Rule> itemsToAdd)
        {
            _dbSyncDataManager.ApplyRouteRulesToTemp(itemsToAdd);
            TotalRowsSuccess = itemsToAdd.Count;
        }
    }
}
