using System;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions.PricingTemplateRate
{
    public class MarginRateCalculationContext : IMarginRateCalculationContext
    {
        #region Fields / Constructors

        private Func<Guid, int?> _getCostCalculationMethodIndex;

        public MarginRateCalculationContext(Func<Guid, int?> getCostCalculationMethodIndex)
        {
            _getCostCalculationMethodIndex = getCostCalculationMethodIndex;
        }

        #endregion

        public ZoneItem ZoneItem { get; set; }

        public int? GetCostCalculationMethodIndex(Guid costCalculationMethodId)
        {
            return _getCostCalculationMethodIndex(costCalculationMethodId);
        }
    }
}