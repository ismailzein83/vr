using System;
using System.Collections.Generic;

namespace TOne.WhS.Deal.Entities
{
    public class VolCommitmentDealItem
    {
        public int ZoneGroupNumber { get; set; }

        public string Name { get; set; }

        public int CountryId { get; set; }

        public List<long> ZoneIds { get; set; }

        public List<VolCommitmentDealItemTier> Tiers { get; set; }
    }
}