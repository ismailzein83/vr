using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Business
{
    public interface IChangedSourceSuppliersReader
    {
        bool UseSourceSupplierId { get; }

        IEnumerable<Supplier> GetChangedSuppliers(ref object updatedHandle);
    }

    public class ExternalSupplierSynchronizer
    {
        public void Synchronize()
        {
            IChangedSourceSuppliersReader changeSuppliersReader = null;
            Object supplierUpdateHandle = GetRecentSupplierUpdateHandle();
            var changedSuppliers = changeSuppliersReader.GetChangedSuppliers(ref supplierUpdateHandle);
            if(changedSuppliers != null)
            {

            }
        }

        private object GetRecentSupplierUpdateHandle()
        {
            throw new NotImplementedException();
        }
    }
}
