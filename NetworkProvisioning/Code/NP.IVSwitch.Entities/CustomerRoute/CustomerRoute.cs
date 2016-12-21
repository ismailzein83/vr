using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public class CustomerRoute
    {
        public string Destination { get; set; }
        public int RouteId { get; set; }
        public decimal Percentage { get; set; }
    }
}
