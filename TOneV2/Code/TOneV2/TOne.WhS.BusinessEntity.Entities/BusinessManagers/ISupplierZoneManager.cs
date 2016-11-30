using System;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
    public interface ISupplierZoneManager : IBEManager
    {
        SupplierZone GetSupplierZone(long supplierZoneId);
    }
}
