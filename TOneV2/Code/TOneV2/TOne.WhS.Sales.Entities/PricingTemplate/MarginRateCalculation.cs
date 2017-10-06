using System;

namespace TOne.WhS.Sales.Entities
{
    public abstract class MarginRateCalculation
    {
        public abstract Guid ConfigId { get; }

        public abstract decimal? GetRate(IMarginRateCalculationContext context);
    }

    public interface IMarginRateCalculationContext
    {
        ZoneItem ZoneItem { get; }

        int? GetCostCalculationMethodIndex(Guid costCalculationMethodConfigId);
    }
}
