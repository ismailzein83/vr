using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups
{
    public class SelectiveSaleZoneGroup : SaleZoneGroupSettings
    {
        public List<long> ZoneIds { get; set; }

        public override IEnumerable<long> GetZoneIds(SaleZoneGroupContext context)
        {
            return this.ZoneIds;
        }

        public override string GetDescription()
        {
            SaleZoneManager manager = new SaleZoneManager();
            return manager.GetDescription(base.SellingNumberPlanId, this.ZoneIds);
        }
    }
}
