using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups
{
    public class AllExceptSaleZoneGroup : SaleZoneGroupSettings
    {
        public List<long> ZoneIds { get; set; }

        public override IEnumerable<long> GetZoneIds(ISaleZoneGroupContext context)
        {
            return this.ZoneIds;
        }

        public override string GetDescription()
        {
            SaleZoneManager manager = new SaleZoneManager();
            StringBuilder builder = new StringBuilder();
            return builder.AppendFormat("All Except: {0}", manager.GetDescription(base.SellingNumberPlanId, this.ZoneIds)).ToString();
        }
    }
}
