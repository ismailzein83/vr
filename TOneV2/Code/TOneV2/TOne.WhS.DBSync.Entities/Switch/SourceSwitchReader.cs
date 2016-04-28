using System.Collections.Generic;
using Vanrise.Entities.EntityMigrator;

namespace TOne.WhS.DBSync.Entities
{
    public abstract class SourceSwitchReader : IMigrationSourceItemReader<SourceSwitch> 
    {
        public abstract IEnumerable<SourceSwitch> GetSourceItems();
    }
}
