using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.RouteSync.Entities
{
    public class TechnicalNumberPlan
    {
        public List<ZoneCode> Codes { get; set; }
    }

    public class ZoneCode
    {
        public string Code { get; set; }
    }
}
