using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public abstract class RateCalculationMethod
    {
        public virtual Guid ConfigId { get; set; }

        public abstract void CalculateRate(IRateCalculationMethodContext context);
    }
}
