using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Sales.Entities
{
   public class TQISupplierInfoQuery
    {
        public RPRouteDetailByZone RPRouteDetail { get; set; }
        public decimal PeriodValue { get; set; }
        public PeriodTypes PeriodType { get; set; }
    }
}
