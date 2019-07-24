using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Sales.Entities
{
    public class TargetMatchImportedDataInput
    {
        public int OwnerId { get; set; }

        public long FileId { get; set; }

        public bool HeaderRowExists { get; set; }

        public RateCalculationMethod RateCalculationMethod { get; set; }

        public Func<long, ZoneItem> GetZoneItem { get; set; }

        public Func<Guid, int?> GetCostCalculationMethodIndex { get; set; }
    }
}
