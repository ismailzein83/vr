using System;

namespace TOne.WhS.BusinessEntity.Entities
{
    public class SupplierZone
    {
        public static Guid BUSINESSENTITY_DEFINITION_ID = new Guid("AD86042C-0B49-4379-966A-DC0D39ADBA6D");

        public long SupplierZoneId { get; set; }

        public int CountryId { get; set; }

        public int SupplierId { get; set; }

        public string Name { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public string SourceId { get; set; }
    }
}
