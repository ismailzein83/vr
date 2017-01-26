using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public class SaleZone : Vanrise.Entities.IDateEffectiveSettings
    {
        public static Guid BUSINESSENTITY_DEFINITION_ID = new Guid("10740F30-5A20-4718-B5AF-0E2E160B21C2");
        public static Guid MASTERPLAN_BUSINESSENTITY_DEFINITION_ID = new Guid("F650D523-7ADB-4787-A2F6-C13168F7E8F7");
        public long SaleZoneId { get; set; }

        public int SellingNumberPlanId { get; set; }

        public int CountryId { get; set; }

        public string Name { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public string SourceId { get; set; }
    }
}
