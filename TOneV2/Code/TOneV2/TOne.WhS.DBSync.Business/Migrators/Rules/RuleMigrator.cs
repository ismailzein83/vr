using System;
using System.Collections.Generic;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Rules;
using Vanrise.Rules.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class RuleMigrator : Migrator<SourceRule, Rule>
    {
        readonly RulesDBSyncDataManager _dbSyncDataManager;
        RuleBaseMigrator _routeRuleBaseMigrator;

        public RuleMigrator(MigrationContext context)
            : base(context)
        {

            _dbSyncDataManager = new RulesDBSyncDataManager(context.UseTempTables);
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
                }
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
                }
                routeRules.AddRange(_routeRuleBaseMigrator.GetSourceRules());
                _routeRuleBaseMigrator.WriteFaildRowsLog();
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
