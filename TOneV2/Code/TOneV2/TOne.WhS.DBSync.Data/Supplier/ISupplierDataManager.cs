using TOne.WhS.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.DBSync.Entities;
using Vanrise.Entities;

namespace TOne.WhS.DBSync.Data
{
    public interface ISupplierDataManager : IDataManager
    {
        List<Supplier> GetSuppliers();

        bool Insert(Supplier supplier);

        bool Update(Supplier supplier);
        void InsertSupplierFromeSource(Supplier supplier);

        void UpdateSupplierFromeSource(Supplier supplier);

        bool AreSuppliersUpdated(ref object updateHandle);
        Supplier GetSupplierBySourceId(string sourceSupplierId);
    }
}
