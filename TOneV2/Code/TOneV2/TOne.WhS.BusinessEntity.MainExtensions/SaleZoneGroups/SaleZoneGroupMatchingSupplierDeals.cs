using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups
{
    public class SaleZoneGroupMatchingSupplierDeals : SaleZoneGroupSettings
    {
        public override Guid ConfigId { get { return new Guid("8A2D2C28-A3B8-48BD-AE0E-0BE382CE6170"); } }

        public List<long> ZoneIds { get; set; }

        public List<int> SupplierDealIds { get; set; }

        public override IEnumerable<long> GetZoneIds(ISaleZoneGroupContext context)
        {
            throw new NotImplementedException();
        }

        public override string GetDescription(ISaleZoneGroupContext context)
        {
            throw new NotImplementedException();
        }

        public override void CleanDeletedZoneIds(ISaleZoneGroupCleanupContext context)
        {
            throw new NotImplementedException();
        }
    }
}
