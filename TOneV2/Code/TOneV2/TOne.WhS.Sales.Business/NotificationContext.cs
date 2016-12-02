using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Sales.Business
{
    public class SalePricelistFileContext : ISalePricelistFileContext
    {
        public int SellingNumberPlanId { get; set; }

        public long ProcessInstanceId { get; set; }

        public IEnumerable<int> CustomerIds { get; set; }

        public IEnumerable<SalePLZoneChange> ZoneChanges { get; set; }

        public DateTime EffectiveDate { get; set; }

        public SalePLChangeType ChangeType { get; set; }

    }
}
