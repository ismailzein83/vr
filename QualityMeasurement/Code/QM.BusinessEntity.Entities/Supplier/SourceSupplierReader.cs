using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Entities.EntitySynchronization;

namespace QM.BusinessEntity.Entities
{
    public abstract class SourceSupplierReader : ISourceItemReader<SourceSupplier> 
    {
        public abstract Guid ConfigId { get; }

        public abstract bool UseSourceItemId
        {
            get;
        }

        public abstract IEnumerable<SourceSupplier> GetChangedItems(ref object updatedHandle);
    }
}
