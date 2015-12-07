using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.MainExtensions.SuppliersReaders
{
    public class SupplierSQLReader : ISourceItemReader<SourceSupplier> 
    {
        public string ConnectionString { get; set; }

        public bool UseSourceItemId
        {
            get;
            set;
        }

        public IEnumerable<SourceSupplier> GetChangedItems(ref object updatedHandle)
        {
            throw new NotImplementedException();
        }
    }
}
