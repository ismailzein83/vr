using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public interface ISaleZoneGroupContext : IContext
    {
        int? RoutingProductId { get; set; }
        IEnumerable<long> GetGroupZoneIds(SaleZoneGroupSettings saleZoneGroup);

        string GetGroupDescription(SaleZoneGroupSettings saleZoneGroup);
    }
}
