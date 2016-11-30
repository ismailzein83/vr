using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public interface ISupplierZoneServiceReader
    {
        SupplierDefaultService GetSupplierDefaultService(int supplierId, DateTime? effectiveOn);

        SupplierZoneService GetSupplierZoneServicesByZone(int supplierId, long supplierZoneId, DateTime? effectiveOn);
    }

    public class SupplierZoneServicesByZone : Dictionary<long, SupplierZoneService>
    {

    }

    public class SupplierZoneServicesByZoneData : Dictionary<long, List<SupplierZoneService>>
    {

    }
}
