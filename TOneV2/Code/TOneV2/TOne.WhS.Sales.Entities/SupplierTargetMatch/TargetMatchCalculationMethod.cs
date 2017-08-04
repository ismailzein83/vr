using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;

namespace TOne.WhS.Sales.Entities
{
    public abstract class TargetMatchCalculationMethod
    {
        public abstract Guid ConfigId { get; }

        public abstract void CalculateRate(ITargetMatchCalculationMethodContext context);
    }
}
