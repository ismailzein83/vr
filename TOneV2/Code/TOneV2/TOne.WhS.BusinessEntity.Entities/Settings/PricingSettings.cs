using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class PricingSettings
    {
        public decimal? DefaultRate { get; set; }
        public decimal? MaximumRate { get; set; }

        public int? EffectiveDateDayOffset { get; set; }
        public int? RetroactiveDayOffset { get; set; }

        public int? NewRateDayOffset { get; set; }
        public int? IncreasedRateDayOffset { get; set; }
        public int? DecreasedRateDayOffset { get; set; }
    }
}
