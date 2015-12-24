using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public interface IRateCalculationMethodContext
    {
        Decimal? Cost { get; }

        Decimal? Rate { set; }
    }
}
