using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public class SaleZone : Vanrise.Entities.IDateEffectiveSettings
    {
        public long SaleZoneId { get; set; }

        public int SellingNumberPlanId { get; set; }

        public int CountryId { get; set; }

        public string Name { get; set; }

        public DateTime BED { get; set; }

        public DateTime? EED { get; set; }

        public string SourceId { get; set; }
    }
}
