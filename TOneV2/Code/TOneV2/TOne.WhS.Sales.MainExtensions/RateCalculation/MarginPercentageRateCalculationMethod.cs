using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.RateCalculation
{
    public class MarginPercentageRateCalculationMethod : RateCalculationMethod
    {
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("84B2B3CD-2CFB-4A54-A619-EC51AE5CFA36"); } }
        public decimal MarginPercentage { get; set; }

        public override void CalculateRate(IRateCalculationMethodContext context)
        {
            if (context != null && context.Cost != null)
            {
                decimal cost = (decimal)context.Cost;
                context.Rate = cost + ((MarginPercentage * cost) / 100);
            }
        }
    }
}
