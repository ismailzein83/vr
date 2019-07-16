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

        public int RoutingDatabaseId { get; set; }

        public Guid PolicyConfigId { get; set; }

        public int? NumberOfOptions { get; set; }

        public RateCalculationMethod RateCalculationMethod { get; set; }

        public IEnumerable<CostCalculationMethod> CostCalculationMethods { get; set; }

        public int CurrencyId { get; set; }

    }
}
