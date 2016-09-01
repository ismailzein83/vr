using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class RuleMigrator : Migrator<SourceRule, RouteRule>
    {
        MigrationContext _migrationContext;
        RulesDBSyncDataManager dbSyncDataManager;
        RouteRuleBaseMigrator _routeRuleBaseMigrator;
        public RuleMigrator(MigrationContext context)
            : base(context)
        {
            _migrationContext = context;
            dbSyncDataManager = new RulesDBSyncDataManager(context.UseTempTables);
            // dataManager = new SourceRouteOverrideRuleDataManager(context.ConnectionString, true);

        }
        public override void FillTableInfo(bool useTempTables)
        {

        }

        public override void AddItems(List<RouteRule> itemsToAdd)
        {
            dbSyncDataManager.ApplyRouteRulesToTemp(itemsToAdd);
            TotalRowsSuccess = itemsToAdd.Count;
        }

        public override IEnumerable<SourceRule> GetSourceItems()
        {
            List<SourceRule> routeRules = new List<SourceRule>();

            _routeRuleBaseMigrator = new RouteOverrideRuleMigrator();
            routeRules.AddRange(_routeRuleBaseMigrator.GetRouteRules(_migrationContext));

            return routeRules;
        }

        public override RouteRule BuildItemFromSource(SourceRule sourceItem)
        {
            return sourceItem.RouteRule;
        }
    }
}
