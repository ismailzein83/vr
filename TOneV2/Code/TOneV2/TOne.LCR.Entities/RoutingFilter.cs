using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class RoutingFilter
    {
        public List<int> ZoneIds { get; set; }
        public string Code { get; set; }
        public List<string> CustomerIds { get; set; }
    }
}
