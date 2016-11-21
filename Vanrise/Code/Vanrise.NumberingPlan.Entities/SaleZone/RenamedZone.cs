using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.NumberingPlan.Entities
{
    public class RenamedZone
    {
        public int CountryId { get; set; }
        public long ZoneId { get; set; }
        public string NewZoneName { get; set; }
    }
}
