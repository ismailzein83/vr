using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Entities;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.DBSync.Business
{
    public abstract class RouteRuleBaseMigrator
    {
        public abstract IEnumerable<SourceRule> GetRouteRules(MigrationContext context);

    }
}
