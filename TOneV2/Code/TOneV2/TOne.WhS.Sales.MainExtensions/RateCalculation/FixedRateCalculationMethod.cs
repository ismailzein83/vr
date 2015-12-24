using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.RateCalculation
{
    public class FixedRateCalculationMethod : RateCalculationMethod
    {
        public decimal FixedRate { get; set; }

        public override void CalculateRate(IRateCalculationMethodContext context)
        {
            if (context != null)
                context.Rate = FixedRate;
        }
    }
}
