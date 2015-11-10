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

        public override string GetDescription(ISaleZoneGroupContext context)
        {           
            var validZoneIds = context.GetGroupZoneIds(this);
            if (validZoneIds != null)
            {
                SaleZoneManager manager = new SaleZoneManager();
                return String.Format("All Except: {0}", manager.GetDescription(base.SellingNumberPlanId, validZoneIds));
            }
            else
                return null;
        }
    }
}
