using System;
using System.Collections.Generic;

namespace TOne.WhS.Deal.Entities
{
    public abstract class SwapDealAnalysisInboundItemRateCalcMethod
    {
        public abstract Guid ConfigId { get; }

        public abstract decimal? Execute(ISwapDealAnalysisInboundRateCalcMethodContext context);
    }

    public interface ISwapDealAnalysisInboundRateCalcMethodContext
    {
        int CustomerId { get; set; }

        int CountryId { get; set; }

        IEnumerable<long> SaleZoneIds { get; set; }
    }
}
