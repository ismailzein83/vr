using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SaleZoneGroupCleanupContext : ISaleZoneGroupCleanupContext
    {
        public SaleZoneGroupCleanupResult Result { get; set; }

        public IEnumerable<long> DeletedSaleZoneIds { get; set; }
    }
}
