using System;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum SaleZoneTypeEnum
    {
        Fixed = 0,
        Mobile = 1
    }
    public class SaleZone : Vanrise.Entities.IDateEffectiveSettings
    {
        public static Guid BUSINESSENTITY_DEFINITION_ID = new Guid("900D0E5D-0FA7-428E-A83B-CD64E16F7415");

        public long SaleZoneId { get; set; }

        public int SellingNumberPlanId { get; set; }

        public int CountryId { get; set; }

        public string Name { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public string SourceId { get; set; }
    }
}
