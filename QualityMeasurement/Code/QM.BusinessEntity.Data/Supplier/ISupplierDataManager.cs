using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QM.BusinessEntity.Data
{
    public interface ISupplierDataManager : IDataManager
    {
        List<Supplier> GetSuppliers();

        bool Insert(Supplier supplier);

        bool Update(Supplier supplier);
        bool Delete(Supplier supplier);
        void InsertSupplierFromeSource(Supplier supplier);

        void UpdateSupplierFromeSource(Supplier supplier);

        bool AreSuppliersUpdated(ref object updateHandle);
        Supplier GetSupplierBySourceId(string sourceSupplierId);
    }
}
