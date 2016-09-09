using System.Collections.Generic;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using Vanrise.Rules;

namespace TOne.WhS.DBSync.Business
{
    public class RuleMigrator : Migrator<SourceRule, BaseRule>
    {
        MigrationContext _migrationContext;
        readonly RulesDBSyncDataManager _dbSyncDataManager;
        RouteRuleBaseMigrator _routeRuleBaseMigrator;


        public RuleMigrator(MigrationContext context)
            : base(context)
        {
            _migrationContext = context;
            _dbSyncDataManager = new RulesDBSyncDataManager(context.UseTempTables);
        }
        public override void FillTableInfo(bool useTempTables)
        {

        }

        public override void AddItems(List<BaseRule> itemsToAdd)
        {
            _dbSyncDataManager.ApplyRouteRulesToTemp(itemsToAdd);
            TotalRowsSuccess = itemsToAdd.Count;
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
            //_routeRuleBaseMigrator = new RouteOverrideRuleMigrator(ruleContext);
            _routeRuleBaseMigrator = new RouteOptionBlockRuleMigrator(ruleContext);
            routeRules.AddRange(_routeRuleBaseMigrator.GetRouteRules());

            return routeRules;
        }

        public override BaseRule BuildItemFromSource(SourceRule sourceItem)
        {
            return sourceItem.RouteRule;
        }
    }
}
