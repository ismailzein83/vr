using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public class NewZoneInput
    {
        public int SellingNumberPlanId { get; set; }
        public List<NewZone> NewZones { get; set; }
    }
}
