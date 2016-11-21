using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public class RenamedZoneInput
    {
        public int CountryId { get; set; }
        public int SellingNumberPlanId { get; set; }
        public long? ZoneId { get; set; }
        public string NewZoneName { get; set; }
        public string OldZoneName { get; set; }
    }
}
