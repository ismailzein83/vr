using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public class DealReprocessInput
    {
        public long DealReprocessInputID { get; set; }

        public int DealID { get; set; }

        public int ZoneGroupNb { get; set; }

        public bool IsSale { get; set; }

        public int TierNb { get; set; }

        public int RateTierNb { get; set; }

        public DateTime FromTime { get; set; }

        public DateTime ToTime { get; set; }

        public decimal UpToDurationInSec { get; set; }

        public DateTime CreatedTime { get; set; }
    }
}
