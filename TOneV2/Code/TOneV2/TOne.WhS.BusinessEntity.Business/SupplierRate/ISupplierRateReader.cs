using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.Business
{
    public interface ISupplierRateReader
    {
        SupplierRatesByZone GetSupplierRates(int supplierId, DateTime? effectiveOn);
    }

    public class SupplierRatesByZone : VRDictionary<long, SupplierZoneRate>
    {

    }
}
