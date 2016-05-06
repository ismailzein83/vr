using System.Collections.Generic;

namespace Vanrise.Entities.EntityMigrator
{
    public interface IMigrationSourceItemReader<T> where T : Vanrise.Entities.EntitySynchronization.ISourceItem
    {
        IEnumerable<T> GetSourceItems();
    }
}
