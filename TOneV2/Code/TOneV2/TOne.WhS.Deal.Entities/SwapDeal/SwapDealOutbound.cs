using System;
using System.Collections.Generic;

namespace TOne.WhS.Deal.Entities
{
    public class SwapDealOutbound
    {
        public int ZoneGroupNumber { get; set; }

        public string Name { get; set; }

        public List<int> CountryIds { get; set; }

        public List<SwapSupplierZone> SupplierZones { get; set; }

        public int Volume { get; set; }

        public Decimal Rate { get; set; }
        public Decimal? ExtraVolumeRate { get; set; }
        public SubstituteRateType SubstituteRateType { get; set; }
        public Decimal? FixedRate { get; set; }
    }



    public class SwapSupplierZone
    {
        public long ZoneId { get; set; }
    }
}
