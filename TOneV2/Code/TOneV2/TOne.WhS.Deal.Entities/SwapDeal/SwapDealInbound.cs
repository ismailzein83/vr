using System;
using System.Collections.Generic;

namespace TOne.WhS.Deal.Entities
{
    public class SwapDealInbound
    {
        public int ZoneGroupNumber { get; set; }

        public string Name { get; set; }

        public int CountryId { get; set; }

        public List<SwapSaleZone> SaleZones { get; set; }

        public int Volume { get; set; }

        public Decimal Rate { get; set; }
        public Decimal? ExtraVolumeRate { get; set; }
        public SubstituteRateType SubstituteRateType { get; set; }
        public Decimal? FixedRate { get; set; }
    }

    public class SwapSaleZone
    {
        public long ZoneId { get; set; }
    }
}