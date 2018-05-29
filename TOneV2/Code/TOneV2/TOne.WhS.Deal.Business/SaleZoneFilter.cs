using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Vanrise.Common;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;
using TOne.WhS.Deal.Entities;

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
            SwapDealManager swapDealManager = new SwapDealManager();
            if (context.SaleZone == null)
                throw new ArgumentNullException("SaleZone");
            return swapDealManager.IsZoneExcluded(context.SaleZone.SaleZoneId, BED, EED, CarrierAccountId, DealId,true);
        }
    }
}
