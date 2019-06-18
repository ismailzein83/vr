using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Deal.Entities;

namespace TOne.WhS.Deal.MainExtensions
{
    public class SwapDealAnalysisInboundItemRateFixed : SwapDealAnalysisInboundItemRateCalcMethod
    {
        public decimal Rate { get; set; }

        public override Guid ConfigId => new Guid("76F467C2-E440-4B11-A42C-59F69FDDBCB7");

        public override decimal? Execute(ISwapDealAnalysisInboundRateCalcMethodContext context)
        {
            return this.Rate;
        }
    }
}
