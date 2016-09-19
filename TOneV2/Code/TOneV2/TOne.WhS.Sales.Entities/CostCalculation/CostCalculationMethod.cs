using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public abstract class CostCalculationMethod
    {
        public virtual Guid ConfigId { get; set; }

        public string Title { get; set; }

        public abstract void CalculateCost(ICostCalculationMethodContext context);
    }
}
