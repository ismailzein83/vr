using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class DealDetailedProgress
    {
        public long DealDetailedProgressID { get; set; }

        public int DealID { get; set; }

        public int ZoneGroupNb { get; set; }

        public bool IsSale { get; set; }

        public int? TierNb { get; set; }

        public int? RateTierNb { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public decimal ReachedDurationInSeconds { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}