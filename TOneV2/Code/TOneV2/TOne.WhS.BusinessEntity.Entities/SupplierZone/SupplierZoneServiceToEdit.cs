using System;
using System.Collections.Generic;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierZoneServiceToEdit
    {
        public long SupplierZoneServiceId { get; set; }
        public string ZoneName { get; set; }
        public long SupplierZoneId { get; set; }
        public int SupplierId { get; set; }
        public DateTime BED { get; set; }
        public DateTime ZoneBED { get; set; }
        public DateTime? ZoneEED { get; set; }
        public List<ZoneService> Services { get; set; }
    }
}
