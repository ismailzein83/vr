using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.Sales.Entities;

namespace TOne.WhS.Sales.Business
{
    public class ApplyBulkActionToZoneItemContext : IApplyBulkActionToZoneItemContext
    {
        public ZoneItem ZoneItem { get; set; }
    }
}
