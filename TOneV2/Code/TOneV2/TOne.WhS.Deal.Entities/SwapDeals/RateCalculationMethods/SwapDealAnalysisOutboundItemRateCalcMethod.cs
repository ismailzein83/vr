using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Deal.Entities
{
    public abstract class SwapDealAnalysisOutboundItemRateCalcMethod
    {
        public Guid CalculationMethodId { get; set; }

        public abstract Decimal Execute(ISwapDealAnalysisOutboundRateCalcMethodContext context);
    }

    public interface ISwapDealAnalysisOutboundRateCalcMethodContext
    {
        SwapDealAnalysisOutboundRateCalcMethod RateCalculationMethod { get; set; }

        int SupplierId { get; }

        List<long> SupplierZoneIds { get; }
    }
}
