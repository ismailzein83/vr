using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.MainExtensions.SourceSuppliersReaders
{
    public class SupplierTOneV2SQLReader : SourceSupplierReader 
    {
        public string ConnectionString { get; set; }

        public override bool UseSourceItemId
        {
            get
            {
                return true;
            }
        }

        public override IEnumerable<SourceSupplier> GetChangedItems(ref object updatedHandle)
        {
            List<SourceSupplier> lst = new List<SourceSupplier>();
            lst.Add(new SourceSupplier
            {
                Name = "TOne V2 test",
                SourceId = "1"
            });
            lst.Add(new SourceSupplier
            {
                Name = "TOne V2",
                SourceId = "2"
            });
            return lst;
        }
    }
}
