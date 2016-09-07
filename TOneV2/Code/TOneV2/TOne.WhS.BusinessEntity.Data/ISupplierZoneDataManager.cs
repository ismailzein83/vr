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
        List<SupplierZone> GetSupplierZones(int supplierId, DateTime effectiveDate);

        List<SupplierZone> GetSupplierZones();
        
        bool AreSupplierZonesUpdated(ref object updateHandle);

        List<SupplierZone> GetSupplierZonesEffectiveAfter(int supplierId, DateTime minimumDate);
    }
}
