using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Data
{
    public interface ISupplierZoneDataManager:IDataManager
    {
        List<SupplierZone> GetSupplierZones(int supplierId, DateTime effectiveDate);

        List<SupplierZone> GetSupplierZones();
        
        bool AreSupplierZonesUpdated(ref object updateHandle);

        List<SupplierZone> GetSupplierZonesEffectiveAfter(int supplierId, DateTime minimumDate);

        List<SupplierZone> GetEffectiveSupplierZones(int supplierId, DateTime? effectiveOn, bool isEffectiveInFuture);
    }
}