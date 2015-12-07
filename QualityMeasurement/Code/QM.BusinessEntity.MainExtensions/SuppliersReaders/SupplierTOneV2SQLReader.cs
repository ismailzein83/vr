using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.MainExtensions.SuppliersReaders
{
    public class SupplierTOneV2SQLReader : SourceSupplierReader 
    {
        public string ConnectionString { get; set; }

        public override bool UseSourceItemId
        {
            get;
            set;
        }

        public override IEnumerable<SourceSupplier> GetChangedItems(ref object updatedHandle)
        {
            throw new NotImplementedException();
        }
    }
}
