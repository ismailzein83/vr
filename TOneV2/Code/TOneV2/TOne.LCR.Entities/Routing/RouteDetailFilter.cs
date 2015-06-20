using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities.Routing
{
    public class RouteDetailFilter
    {
        public IEnumerable<string> CustomerIds { get; set; }
        public IEnumerable<int> ZoneIds { get; set; }
        public string Code { get; set; }
    }
}
