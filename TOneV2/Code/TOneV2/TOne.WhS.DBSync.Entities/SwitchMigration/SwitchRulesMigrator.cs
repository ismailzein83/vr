using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.DBSync.Entities
{
    public abstract class SwitchRulesMigrator
    {
        public SwitchMigrationContext Context { get; set; }
        /// <summary>
        /// Parse Switch Configuration and set Context.SwitchMappings
        /// </summary>
        public abstract void Execute();
    }

    public class SwitchMigrationContext
    {
        public SourceSwitch SourceSwitch { get; set; }
        public IEnumerable<SwitchMapping> SwitchMappings { get; set; }
        public int MappingRuleTypeId { get; set; }
        public int NormalizationRuleTypeId { get; set; }
        public RuleMigrationContext MigrationContext { get; set; }
    }
}
