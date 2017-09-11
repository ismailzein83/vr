using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.SupplierTargetMatchCalculation
{
    public class WeightedAverageTargetMatchCalculation : TargetMatchCalculationMethod
    {
        public override Guid ConfigId
        {
            get { return new Guid("F8C397CD-1930-4E02-811F-DE2CB203EB85"); }
        }

        public override void Evaluate(ITargetMatchCalculationMethodContext context)
        {
            
        }
    }
}
