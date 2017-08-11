using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Routing.Entities;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.SupplierTargetMatchCalculation
{
    public class AverageTargetMatchCalculation : TargetMatchCalculationMethod
    {
        public override Guid ConfigId
        {
            get { return new Guid("1BA9A5CA-BEB1-4071-B7D1-A8C19227CFBA"); }
        }

        public override void Evaluate(ITargetMatchCalculationMethodContext context)
        {
            
        }

        private decimal EvaluateRate(decimal originalRate, ITargetMatchCalculationMethodContext context)
        {
            decimal value = 0;
            switch (context.MarginType)
            {
                case MarginType.Percentage:
                    value = originalRate * (100 - context.MarginValue) / 100;
                    break;
                case MarginType.Fixed:
                    value = context.MarginValue;
                    break;
            }
            return value;
        }
    }
}
