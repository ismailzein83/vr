using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Business;
using TOne.WhS.BusinessEntity.Entities;
using Vanrise.Common;

namespace TOne.WhS.BusinessEntity.MainExtensions.SaleZoneGroups
{
    public class AllExceptSaleZoneGroup : SaleZoneGroupSettings
    {
        public List<long> ZoneIds { get; set; }

        public override IEnumerable<long> GetZoneIds(ISaleZoneGroupContext context)
        {
            SaleZoneManager manager = new SaleZoneManager();
            IEnumerable<SaleZone> saleZonesofSellingNumberPlan = manager.GetSaleZonesBySellingNumberPlan(base.SellingNumberPlanId);
            
            IEnumerable<long> allExceptZoneIds = null;

            if (saleZonesofSellingNumberPlan != null)
            {
                IEnumerable<SaleZone> allExceptSaleZoneList = saleZonesofSellingNumberPlan.FindAllRecords(x => !this.ZoneIds.Contains(x.SaleZoneId));
                if(allExceptSaleZoneList != null)
                {
                    allExceptZoneIds = allExceptSaleZoneList.Select(x => x.SaleZoneId);
                }
            }

            return allExceptZoneIds;
        }

        public override string GetDescription(ISaleZoneGroupContext context)
        {           
            var validZoneIds = context != null ? context.GetGroupZoneIds(this) : this.ZoneIds;
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
