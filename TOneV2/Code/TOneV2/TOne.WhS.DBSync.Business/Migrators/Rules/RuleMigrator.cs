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
        internal static DateTime s_defaultRuleBED = DateTime.Parse("2000-01-01");
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
                    case RuleEntitiesEnum.RouteBlock:
                        _routeRuleBaseMigrator = new RouteBlockRuleMigrator(ruleContext);
                        break;
                    case RuleEntitiesEnum.Tariff:
                        _routeRuleBaseMigrator = new TariffRuleMigrator(ruleContext);
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
