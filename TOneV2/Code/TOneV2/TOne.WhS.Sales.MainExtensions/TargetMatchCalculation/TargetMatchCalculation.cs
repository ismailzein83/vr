using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.TargetMatchCalculation
{
    public class LCR1TargetMatchCalculation : TargetMatchCalculationMethod
    {
        public override Guid ConfigId
        {
            get { return new Guid("1BA9A5CA-BEB1-4071-B7D1-A8C19227CFBA"); }
        }

        public override void CalculateRate(ITargetMatchCalculationMethodContext context)
        {
            throw new NotImplementedException();
        }
    }
}
