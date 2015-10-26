using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups
{
    public class AllExceptSaleZoneGroup : SaleZoneGroupSettings
    {
        public List<long> ZoneIds { get; set; }

        public override IEnumerable<long> GetZoneIds(SaleZoneGroupContext context)
        {
            return this.ZoneIds;
        }

        public override string GetDescription()
        {
            if (this.ZoneIds != null)
                return string.Join(",", this.ZoneIds);

            return string.Empty;
        }
    }
}
