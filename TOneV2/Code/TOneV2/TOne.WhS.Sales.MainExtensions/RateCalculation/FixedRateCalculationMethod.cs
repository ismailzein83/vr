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
        Guid _configId;
        public override Guid ConfigId { get { return _configId; } set { _configId = new Guid("AD64C73F-4F20-447A-93C8-6C0FBCE85E37"); } }
        public decimal FixedRate { get; set; }

        public override void CalculateRate(IRateCalculationMethodContext context)
        {
            if (context != null)
                context.Rate = FixedRate;
        }
    }
}
