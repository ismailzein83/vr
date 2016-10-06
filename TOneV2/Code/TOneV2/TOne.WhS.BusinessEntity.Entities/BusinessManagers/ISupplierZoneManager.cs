using System;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
    public interface ISupplierZoneManager : IBEManager
    {
        List<long> GetSupplierZoneIdsByDates(int supplierId, DateTime fromDate, DateTime? toDate);
    }
}
