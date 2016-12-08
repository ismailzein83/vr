using QM.BusinessEntity.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Data.SQL;

namespace QM.BusinessEntity.Data.SQL
{
    public class SupplierDataManager : BaseSQLDataManager, ISupplierDataManager
    {
        public SupplierDataManager() :
            base(GetConnectionStringName("QM_BE_DBConnStringKey", "QM_BE_DBConnString"))
        {

        }

        public Supplier SupplierMapper(IDataReader reader)
        {
            Supplier supplier = new Supplier()
            {
                SupplierId = (int)reader["ID"],
                Name = reader["Name"] as string,
                SourceId = reader["SourceSupplierID"] as string,
                Settings = Vanrise.Common.Serializer.Deserialize<SupplierSettings>(reader["Settings"] as string)
            };
            return supplier;
        }

        public List<Supplier> GetSuppliers()
        {
            return GetItemsSP("[QM_BE].[sp_Supplier_GetAll]", SupplierMapper);
        }

        public bool Delete(Supplier supplier)
        {
            int recordsEffected = ExecuteNonQuerySP("[QM_BE].[sp_Supplier_Delete]", supplier.SupplierId);
            return (recordsEffected > 0);
        }

        public bool Insert(Supplier supplier)
        {
            object settings = null;
            if (supplier.Settings != null)
                settings = Vanrise.Common.Serializer.Serialize(supplier.Settings);
            int recordsEffected = ExecuteNonQuerySP("[QM_BE].[sp_Supplier_Insert]", supplier.SupplierId, supplier.Name, settings);
            return (recordsEffected > 0);
        }
        public Supplier GetSupplierBySourceId(string sourceSupplierId)
        {
            return GetItemSP("[QM_BE].[sp_Supplier_GetBySourceID]", SupplierMapper, sourceSupplierId);
        }

        public bool Update(Supplier supplier)
        {
             object settings = null;
             if ( supplier.Settings != null)
               settings =  Vanrise.Common.Serializer.Serialize(supplier.Settings);
             int recordsEffected = ExecuteNonQuerySP("[QM_BE].[sp_Supplier_Update]", supplier.SupplierId, supplier.Name, settings, supplier.IsDeleted);
            return (recordsEffected > 0);
        }

        public void InsertSupplierFromeSource(Supplier supplier)
        {
            object settings = null;
            if (supplier.Settings != null)
                settings = Vanrise.Common.Serializer.Serialize(supplier.Settings);

            ExecuteNonQuerySP("[QM_BE].[sp_Supplier_InsertFromSource]", supplier.SupplierId, supplier.Name, settings, supplier.SourceId);
        }

        public void UpdateSupplierFromeSource(Supplier supplier)
        {
           
           ExecuteNonQuerySP("[QM_BE].[sp_Supplier_UpdateFromSource]", supplier.SupplierId, supplier.Name);
        }
        public bool AreSuppliersUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("QM_BE.Supplier", ref updateHandle);
        }
    }
}
