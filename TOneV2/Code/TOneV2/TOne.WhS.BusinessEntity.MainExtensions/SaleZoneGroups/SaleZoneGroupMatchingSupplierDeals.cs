using System;
using System.Collections.Generic;
using TOne.WhS.BusinessEntity.Business;
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
            return this.ZoneIds != null ? this.ZoneIds : new List<long>();
        }

        public override string GetDescription(ISaleZoneGroupContext context)
        {
            var validZoneIds = context != null ? context.GetGroupZoneIds(this) : this.ZoneIds;
            if (validZoneIds != null)
            {
                SaleZoneManager manager = new SaleZoneManager();
                return manager.GetDescription(base.SellingNumberPlanId, validZoneIds);
            }
            else
                return null;
        }

        public override void CleanDeletedZoneIds(ISaleZoneGroupCleanupContext context)
        {
            context.Result = SaleZoneGroupCleanupResult.NoChange;

            if (this.ZoneIds != null && this.ZoneIds.Count > 0)
            {
                foreach (int deletedZoneId in context.DeletedSaleZoneIds)
                {
                    if (this.ZoneIds.Contains(deletedZoneId))
                    {
                        context.Result = SaleZoneGroupCleanupResult.ZonesUpdated;
                        this.ZoneIds.Remove(deletedZoneId);
                    }
                }

                if (this.ZoneIds.Count == 0)
                    context.Result = SaleZoneGroupCleanupResult.AllZonesRemoved;
            }
        }
    }
}