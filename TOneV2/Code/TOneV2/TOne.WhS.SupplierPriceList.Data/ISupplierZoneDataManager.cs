using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;

namespace TOne.WhS.SupplierPriceList.Data
{
    public interface ISupplierZoneDataManager:IDataManager
    {
        void InsertSupplierZones(List<Zone> supplierZones);
        bool UpdateSupplierZones(int supplierId, DateTime effectiveDate);
    }
}
