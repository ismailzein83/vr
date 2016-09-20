using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.RateCalculation
{
    public class MarginRateCalculationMethod : RateCalculationMethod
    {
        public override Guid ConfigId { get { return  new Guid("9848AF1F-0C8A-4236-B5CC-81D593B07B85"); } }
        public decimal Margin { get; set; }

        public override void CalculateRate(IRateCalculationMethodContext context)
        {
            if (context != null && context.Cost != null)
                context.Rate = (decimal)context.Cost + Margin;
        }
    }
}
