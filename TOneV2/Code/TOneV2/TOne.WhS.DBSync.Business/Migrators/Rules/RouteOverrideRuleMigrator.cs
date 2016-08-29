using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Data.SQL;
using TOne.WhS.DBSync.Data.SQL.SourceDataManger;
using TOne.WhS.DBSync.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.DBSync.Business
{
    public class RouteOverrideRuleMigrator : RouteRuleBaseMigrator
    {
        public override IEnumerable<SourceRule> GetRouteRules(MigrationContext context)
        {
            List<SourceRule> routeRules = new List<SourceRule>();

            SourceRouteOverrideRuleDataManager dataManager = new SourceRouteOverrideRuleDataManager(context.ConnectionString, true);
            var overrideRules = dataManager.GetRouteOverrideRules();

            return routeRules;
        }
    }
}
