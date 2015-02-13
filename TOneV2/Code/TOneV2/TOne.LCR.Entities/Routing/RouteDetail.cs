using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class RouteDetail : Route
    {
        public int ZoneId { get; set; }

        public decimal Rate { get; set; }

        public int ServicesFlag { get; set; }

        public RouteOptionsDetail Options { get; set; }
    }
}
