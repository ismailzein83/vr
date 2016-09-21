using System;
using System.Collections.Generic;
using Vanrise.Entities.EntitySynchronization;

namespace QM.CLITester.Entities
{
    public abstract class SourceProfileReader : ISourceItemReader<SourceProfile> 
    {
        public abstract Guid ConfigId { get; }

        public abstract bool UseSourceItemId
        {
            get;
        }

        public abstract IEnumerable<SourceProfile> GetChangedItems(ref object updatedHandle);
    }
}
