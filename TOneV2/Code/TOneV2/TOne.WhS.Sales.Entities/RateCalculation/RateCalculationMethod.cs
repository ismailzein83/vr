using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public abstract class RateCalculationMethod
    {
        public abstract Guid ConfigId { get; }

        public abstract void CalculateRate(IRateCalculationMethodContext context);
    }
}
