﻿using System;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class PricingSettings
    {
        public decimal? DefaultRate { get; set; }
        public decimal? MaximumRate { get; set; }
        public int? EffectiveDateDayOffset { get; set; }
        public int? RetroactiveDayOffset { get; set; }
        public int? NewRateDayOffset { get; set; }
        public int? EndCountryDayOffset { get; set; }
        public int? IncreasedRateDayOffset { get; set; }
        public int? DecreasedRateDayOffset { get; set; }
        public bool? AllowRateZero { get; set; }
    }
}
