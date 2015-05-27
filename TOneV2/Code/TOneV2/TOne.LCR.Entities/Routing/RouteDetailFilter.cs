using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities.Routing
{
    public class RouteDetailFilter
    {
        public List<string> CustomerIds { get; set; }
        public List<int> ZoneIds { get; set; }
        public List<string> Code { get; set; }
    }
}
