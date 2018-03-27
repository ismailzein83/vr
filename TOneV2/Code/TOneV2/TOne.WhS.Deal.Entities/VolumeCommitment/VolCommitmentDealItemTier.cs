using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class VolCommitmentDealItemTier
    {
        public int? UpToVolume { get; set; }

        public int? RetroActiveFromTierNumber { get; set; }

        public BaseDealRateEvaluator EvaluatedRate { get; set; }

        public string Description { get { return this.EvaluatedRate.GetDescription(); } }

        public IEnumerable<VolCommitmentDealItemTierZoneRate> ExceptionZoneRates { get; set; }

    }

    public class VolCommitmentDealItemTierZoneRate
    {
        public IEnumerable<VolSaleZone> Zones { get; set; }

        public BaseDealRateEvaluator EvaluatedRate { get; set; }
        public string Description { get { return this.EvaluatedRate.GetDescription(); } }
    }
}
