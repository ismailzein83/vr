using System;

namespace TOne.WhS.Deal.Entities
{
    public class SwapDealBuyRouteRuleDetails
    {
        public long VRRuleId { get; set; }

        public int SwapDealId { get; set; }

        public string Description { get; set; }

        public string RuleType { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public string SupplierZonesDescription { get; set; }

        public string Settings { get; set; }
    }
}