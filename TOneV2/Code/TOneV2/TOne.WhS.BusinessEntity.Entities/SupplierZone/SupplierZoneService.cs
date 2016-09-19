using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum ZoneServiceChangeType
    {

        [Description("Not Changed")]
        NotChanged = 0,

        [Description("New")]
        New = 1,

        [Description("Deleted")]
        Deleted = 2,
    }
    public class SupplierZoneService
    {
        public long SupplierZoneServiceId { get; set; }

        public long ZoneId { get; set; }

        public int PriceListId { get; set; }

        public List<ZoneService> ReceivedServices { get; set; }

        public List<ZoneService> EffectiveServices { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public string SourceId { get; set; }

    }
}
