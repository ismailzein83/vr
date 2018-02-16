using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Sales.Entities
{
    public class TQIEvaluatedRateInput
    {
        public TQIMethod TQIMethod { get; set; }
        public RPRouteDetailByZone RPRouteDetail { get; set; }
    }
}
