using System;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierZoneService
    {
        public long SupplierZoneServiceId { get; set; }

        public long ZoneId { get; set; }

        public List<ZoneService> ReceivedServices { get; set; }

        public List<ZoneService> EffectiveServices { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

    }
}
