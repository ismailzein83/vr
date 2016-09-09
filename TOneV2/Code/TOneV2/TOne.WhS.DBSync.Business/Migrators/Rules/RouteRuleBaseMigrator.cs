using System.Collections.Generic;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public abstract class RouteRuleBaseMigrator
    {
        public RuleMigrationContext Context { get; set; }
        public abstract IEnumerable<SourceRule> GetRouteRules();

        protected RouteRuleBaseMigrator(RuleMigrationContext context)
        {
            Context = context;
        }
    }
}
