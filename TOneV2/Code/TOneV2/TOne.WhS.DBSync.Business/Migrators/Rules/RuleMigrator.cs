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
        RuleBaseMigrator _rulesBaseMigrator;
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
                        _rulesBaseMigrator = new RouteOverrideRuleMigrator(ruleContext);
                        break;
                    case RuleEntitiesEnum.RouteOptionBlock:
                        _rulesBaseMigrator = new RouteOptionBlockRuleMigrator(ruleContext);
                        break;
                    case RuleEntitiesEnum.Commission:
                        _rulesBaseMigrator = new CommissionRuleMigrator(ruleContext);
                        break;
                    case RuleEntitiesEnum.SaleMarketPrice:
                        _rulesBaseMigrator = new MarketPriceRuleMigrator(ruleContext);
                        break;
                    case RuleEntitiesEnum.RouteBlock:
                        _rulesBaseMigrator = new RouteBlockRuleMigrator(ruleContext);
                        break;
                    case RuleEntitiesEnum.Tariff:
                        _rulesBaseMigrator = new TariffRuleMigrator(ruleContext);
                        break;
                    case RuleEntitiesEnum.Tod:
                        _rulesBaseMigrator = new CommissionRuleMigrator(ruleContext);
                        break;
                    case RuleEntitiesEnum.SwitchRules:
                        _rulesBaseMigrator = new SwitchRuleMigrator(ruleContext);
                        break;
                    default:
                        _rulesBaseMigrator = null;
                        break;
                }
                if (_rulesBaseMigrator != null)
                {
                    routeRules.AddRange(_rulesBaseMigrator.GetSourceRules());
                    _rulesBaseMigrator.WriteFaildRowsLog();
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
