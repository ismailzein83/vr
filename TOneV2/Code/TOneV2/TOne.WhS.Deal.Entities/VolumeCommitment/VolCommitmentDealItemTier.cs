using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class VolCommitmentDealItemTier
    {
        public int StartingVolume { get; set; }

        public int? RetroActivePricingVolume { get; set; }

        public Decimal? DefaultRate { get; set; }

        public VolCommitmentDealItemTierZoneRate ExceptionZoneRates { get; set; }
    }

    public class VolCommitmentDealItemTierZoneRate
    {
        public List<long> ZoneIds { get; set; }

        public Decimal Rate { get; set; }
    }
}
