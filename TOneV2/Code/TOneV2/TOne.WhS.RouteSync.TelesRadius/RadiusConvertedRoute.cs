using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.RouteSync.Entities;

namespace TOne.WhS.RouteSync.TelesRadius
{
    public class RadiusConvertedRoute : ConvertedRoute
    {
        public string CustomerId { get; set; }
        public string Code { get; set; }
        public string Options { get; set; }
    }
}
