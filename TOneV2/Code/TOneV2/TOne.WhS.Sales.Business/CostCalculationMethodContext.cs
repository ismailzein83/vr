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
        public Routing.Entities.RPRouteDetail Route
        {
            get;
            set;
        }

        public decimal Cost
        {

            get;
            set;
        }
    }
}
