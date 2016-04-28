using System.Collections.Generic;
using Vanrise.Entities.EntityMigrator;

namespace TOne.WhS.DBSync.Entities
{
    public abstract class SourceMigrationReader : IMigrationSourceItemReader<SourceSwitch> 
    {
        public abstract IEnumerable<SourceSwitch> GetSourceItems();
    }
}
