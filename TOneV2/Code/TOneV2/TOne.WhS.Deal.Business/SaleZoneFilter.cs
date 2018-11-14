using System;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Deal.Business
{
    public class SaleZoneFilter : ISaleZoneFilter
    {
        public int CarrierAccountId { get; set; }
        public int? DealId { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
        public bool IsExcluded(ISaleZoneFilterContext context)
        {
            DealDefinitionManager dealDefinitionManager = new DealDefinitionManager();
            if (context.SaleZone == null)
                throw new ArgumentNullException("SaleZone");
            return dealDefinitionManager.IsZoneExcluded(context.SaleZone.SaleZoneId, BED, EED, CarrierAccountId, DealId, true);
        }
    }
}
