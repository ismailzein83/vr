using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NP.IVSwitch.Entities
{
    public class RouteTableDetails
    {
        public int RouteTableId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int? PScore { get; set; }
    }
}

