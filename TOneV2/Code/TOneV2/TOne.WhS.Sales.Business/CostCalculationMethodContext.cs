using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class CostCalculationMethodContext : ICostCalculationMethodContext
    {
        public IEnumerable<long> ZoneIds { get; set; }

        public Routing.Entities.RPRouteDetailByZone Route { get; set; }

        public int? NumberOfOptions { get; set; }

        public decimal Cost { get; set; }

        public object CustomObject { get; set; }       
    }
}
