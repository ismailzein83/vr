using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class RateCalculationMethodContext : IRateCalculationMethodContext
    {
        #region Fields / Constructors

        private Func<Guid, int?> _getCostCalculationMethodIndex;

        public RateCalculationMethodContext(Func<Guid, int?> getCostCalculationMethodIndex)
        {
            _getCostCalculationMethodIndex = getCostCalculationMethodIndex;
        }

        #endregion

        public ZoneItem ZoneItem { get; set; }

        public int? GetCostCalculationMethodIndex(Guid costCalculationMethodId)
        {
            return _getCostCalculationMethodIndex(costCalculationMethodId);
        }

        public decimal? Rate { get; set; }
    }
}
