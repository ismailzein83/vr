using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Entities
{
    public abstract class SourceSupplierReader : ISourceItemReader<SourceSupplier> 
    {
        public int ConfigId { get; set; }

        public abstract bool UseSourceItemId
        {
            get;
        }

        public abstract IEnumerable<SourceSupplier> GetChangedItems(ref object updatedHandle);
    }
}
