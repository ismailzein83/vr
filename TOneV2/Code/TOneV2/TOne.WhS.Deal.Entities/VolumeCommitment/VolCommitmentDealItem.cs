using System;
using System.Collections.Generic;

namespace TOne.WhS.Deal.Entities
{
    public class VolCommitmentDealItem
    {
        public int ZoneGroupNumber { get; set; }

        public string Name { get; set; }

        public List<int> CountryIds { get; set; }

        public List<VolSaleZone> SaleZones { get; set; }

        public List<VolCommitmentDealItemTier> Tiers { get; set; }
    }

    public class VolSaleZone
    {
        public long ZoneId { get; set; }
    }
}