using System.Collections.Generic;
using TOne.WhS.DBSync.Entities;

namespace TOne.WhS.DBSync.Business
{
    public abstract class RouteRuleBaseMigrator
    {
        public RuleMigrationContext Context { get; set; }
        public abstract IEnumerable<SourceRule> GetRouteRules();
        public int TotalRowsFailed { get; set; }
        public virtual string EntityName { get { return "RouteRuleBaseMigrator"; } }
        public virtual void WriteFaildRowsLog()
        {
            if (TotalRowsFailed > 0)
                Context.MigrationContext.WriteWarning(string.Format("Migrating table '" + EntityName + "' : {0} rows failed", TotalRowsFailed));
        }

        protected RouteRuleBaseMigrator(RuleMigrationContext context)
        {
            TotalRowsFailed = 0;
            Context = context;
        }
    }
}
