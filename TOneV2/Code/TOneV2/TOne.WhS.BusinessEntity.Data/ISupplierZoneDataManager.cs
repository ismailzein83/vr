using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISupplierZoneDataManager:IDataManager
    {
        void InsertSupplierZones(List<SupplierZone> supplierZones);
        List<SupplierZone> GetSupplierZones(int supplierId, DateTime effectiveDate);
        bool UpdateSupplierZones(int supplierId, DateTime effectiveDate);
    }
}
