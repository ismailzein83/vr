using System;
using System.Collections.Generic;

namespace TOne.WhS.Deal.Entities
{
    public class DealBuyRouteRuleSettings : Vanrise.Entities.VRRuleSettings
    {
        public string Description { get; set; }

        public int DealId { get; set; }

        public List<long> SupplierZoneIds { get; set; }

        public DealBuyRouteRuleExtendedSettings ExtendedSettings { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }
    }
}
