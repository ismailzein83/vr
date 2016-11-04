using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public interface ISupplierRateReader
    {
        SupplierRatesByZone GetSupplierRates(int supplierId, DateTime effectiveOn);
    }

    public class SupplierRatesByZone : Dictionary<long, SupplierZoneRate>
    {

    }
}
