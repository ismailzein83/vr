using System;
using System.Collections.Generic;

namespace TOne.WhS.Deal.Entities
{
    public class SwapDealSettingsDetail
    {
        public int SwapDealId { get; set; }
        public int CarrierAccountId { get; set; }
        public int SellingNumberPlanId { get; set; }
        public List<long> SaleZoneIds { get; set; }
        public List<long> SupplierZoneIds { get; set; }
        public DateTime BED { get; set; }
        public DateTime? EED { get; set; }
    }
}