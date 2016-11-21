using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public class GetCodeItemInput
    {
        public int SellingNumberPlanId { get; set; }
        public long? ZoneId { get; set; }
        public string ZoneName { get; set; }
        public int CountryId { get; set; }
    }
}
