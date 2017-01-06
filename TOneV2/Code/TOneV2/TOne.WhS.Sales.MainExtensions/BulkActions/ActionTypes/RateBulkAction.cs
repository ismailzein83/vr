using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.MainExtensions
{
    public class RateBulkAction : BulkActionType
    {
        public Guid? RateCalculationCostColumnConfigId { get; set; }

        public RateCalculationMethod RateCalculationMethod { get; set; }

        public override bool IsZoneApplicable(long zoneId)
        {
            return true;
        }
    }
}
