using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class RateCalculationMethodContext : IRateCalculationMethodContext
    {
        public decimal? Cost { get; set; }

        public decimal? Rate { get; set; }
    }
}
