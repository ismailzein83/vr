using System;
using System.Collections.Generic;
using System.Linq;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.SellingNumberPlan
{
    public class SelectiveSellingNumberPlan : SaleZoneGroupSettings
    {
        public override Guid ConfigId { get { return new Guid("d566ce83-0010-42ab-88bc-b8b3eaf5b556"); } }
        public List<int> SellingNumberPlanIds { get; set; }
        public override IEnumerable<long> GetZoneIds(ISaleZoneGroupContext context)
        {
            List<long> zoneIds = new List<long>();
            SaleZoneManager manager = new SaleZoneManager();
            foreach (int sellingNumberPlanId in SellingNumberPlanIds)
            {
                IEnumerable<SaleZone> saleZonesofSellingNumberPlan = manager.GetSaleZonesBySellingNumberPlan(sellingNumberPlanId);
                if (saleZonesofSellingNumberPlan != null && saleZonesofSellingNumberPlan.Count() > 0)
                    zoneIds.AddRange(saleZonesofSellingNumberPlan.Select(itm => itm.SaleZoneId));
            }
            return zoneIds.Count == 0 ? null : zoneIds;
        }

        public override string GetDescription(ISaleZoneGroupContext context)
        {
            SellingNumberPlanManager manager = new SellingNumberPlanManager();
            return string.Format("Sale Zones of following Number Plans: {0}", manager.GetDescription(SellingNumberPlanIds));
        }
    }

    public class SelectiveSellingNumberPlanTemplateConfigSettings
    {
        public bool IsHiddenInRP { get; set; }
    }
}