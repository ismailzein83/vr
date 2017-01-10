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
        public override Guid ConfigId { get { return new Guid("9d83276e-92cd-42e5-9f36-1f9c56ed8a3f"); } }
        public List<long> ZoneIds { get; set; }

        public override IEnumerable<long> GetZoneIds(ISaleZoneGroupContext context)
        {
            SaleZoneManager manager = new SaleZoneManager();
            IEnumerable<SaleZone> saleZonesofSellingNumberPlan = manager.GetSaleZonesBySellingNumberPlan(base.SellingNumberPlanId);
            
            List<long> allExceptZoneIds = new List<long>();

            if (saleZonesofSellingNumberPlan != null)
            {
                IEnumerable<SaleZone> allExceptSaleZoneList = saleZonesofSellingNumberPlan.FindAllRecords(x => this.ZoneIds == null || !this.ZoneIds.Contains(x.SaleZoneId));
                if(allExceptSaleZoneList != null)
                {
                    allExceptZoneIds = allExceptSaleZoneList.Select(x => x.SaleZoneId).ToList();
                }
            }

            return allExceptZoneIds;
        }

        public override string GetDescription(ISaleZoneGroupContext context)
        {           
            var validZoneIds = this.ZoneIds;
            if (validZoneIds != null)
            {
                SaleZoneManager manager = new SaleZoneManager();
                return String.Format("All Except: {0}", manager.GetDescription(base.SellingNumberPlanId, validZoneIds));
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
            }
        }
    }
}
