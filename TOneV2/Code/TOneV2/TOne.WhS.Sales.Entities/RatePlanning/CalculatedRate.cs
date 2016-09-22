using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class CalculatedZoneRate
    {
        public long ZoneId { get; set; }
        public string ZoneName { get; set; }
        public decimal? CurrentRate { get; set; }
        public decimal CalculatedRate { get; set; }
    }

    public class CalculatedRates
    {
        public List<CalculatedZoneRate> ValidCalculatedRates { get; set; }
        public List<CalculatedZoneRate> InvalidCalculatedRates { get; set; }
    }
}
