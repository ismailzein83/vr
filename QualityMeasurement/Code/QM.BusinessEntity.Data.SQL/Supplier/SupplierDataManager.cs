﻿using QM.BusinessEntity.Entities;
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
        public SupplierDataManager()
            : base("MainDBConnString")
        {
        }

        public Supplier SupplierMapper(IDataReader reader)
        {
            Supplier supplier = new Supplier()
            {
                SupplierId = (int)reader["ID"],
                Name = reader["Name"] as string
            };
            return supplier;
        }

        public List<Supplier> GetSuppliers()
        {
            return GetItemsSP("[QM_BE].[sp_Supplier_GetAll]", SupplierMapper);
        }

        public bool Insert(Supplier supplier)
        {
            int recordsEffected = ExecuteNonQuerySP("[QM_BE].[sp_Supplier_Insert]", supplier.SupplierId, supplier.Name);
            return (recordsEffected > 0);
        }

        public bool Update(Supplier supplier)
        {
            int recordsEffected = ExecuteNonQuerySP("[QM_BE].[sp_Supplier_Update]", supplier.SupplierId, supplier.Name);
            return (recordsEffected > 0);
        }

        public bool AreSuppliersUpdated(ref object updateHandle)
        {
            return base.IsDataUpdated("QM_BE.Supplier", ref updateHandle);
        }
    }
}
