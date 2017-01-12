using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class ApplicableZoneIdsContext : IApplicableZoneIdsContext
    {
        public IEnumerable<long> SaleZoneIds { get; set; }

        public Changes DraftData { get; set; }

        public BulkActionType BulkAction { get; set; }
    }
}
