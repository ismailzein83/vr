using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.DBSync.Business.Migrators;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Common.Business;
using Vanrise.Entities;
using Vanrise.Rules.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class RuleMigrator : Migrator<SourceRule, Rule>
    {
        readonly RulesDBSyncDataManager _dbSyncDataManager;
        RuleBaseMigrator _rulesBaseMigrator;
        CurrencySettingData _currencySettingData;
        readonly Dictionary<string, CodeGroup> _allCodeGroups;

        internal static DateTime s_defaultRuleBED = DateTime.Parse("2000-01-01");

        public RuleMigrator(MigrationContext context)
            : base(context)
        {
            TableName = "Rules";
            _dbSyncDataManager = new RulesDBSyncDataManager(context.UseTempTables);

            SettingManager settingManager = new SettingManager();
            var _systemCurrencySetting = settingManager.GetSettingByType("VR_Common_BaseCurrency");
            _currencySettingData = (CurrencySettingData)_systemCurrencySetting.Data;

            var dtTableCodeGroups = Context.DBTables[DBTableName.CodeGroup];
            _allCodeGroups = (Dictionary<string, CodeGroup>)dtTableCodeGroups.Records;           
            RulesMigrationHelper.BuildSingleCodeGroupContriesCodeGroups(_allCodeGroups);
        }

        public override void FillTableInfo(bool useTempTables)
        {

        }

        public override IEnumerable<SourceRule> GetSourceItems()
        {
            List<SourceRule> routeRules = new List<SourceRule>();

            RuleMigrationContext ruleContext = new RuleMigrationContext
            {
                MigrationContext = Context,
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
                        _rulesBaseMigrator = new TodRuleMigrator(ruleContext);
                        break;
                    //case RuleEntitiesEnum.SwitchRules:
                    //    _rulesBaseMigrator = new SwitchRuleMigrator(ruleContext);
                    //    break;
                    case RuleEntitiesEnum.RouteOptionBlockFromOverride:
                        _rulesBaseMigrator = new RouteOptionBlockFromOverrideRuleMigrator(ruleContext);
                        break;
                    case RuleEntitiesEnum.SpecialRequest:
                        _rulesBaseMigrator = new SpecialRequestRuleMigrator(ruleContext);
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
            RulesMigrationHelper.ClearSingleCodeGroupContriesCodeGroups();
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