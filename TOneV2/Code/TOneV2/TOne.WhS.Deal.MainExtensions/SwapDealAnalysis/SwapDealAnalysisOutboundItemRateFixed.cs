using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.MainExtensions
{
    public class SwapDealAnalysisOutboundItemRateFixed : SwapDealAnalysisOutboundItemRateCalcMethod
    {
        public Decimal Rate { get; set; }

        public override decimal Execute(ISwapDealAnalysisOutboundRateCalcMethodContext context)
        {
            return this.Rate;
        }
    }
}
