using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Demo.Module.Entities
{
    public interface ISaleZoneGroupContext : IContext
    {
        SaleZoneFilterSettings FilterSettings { get; set; }
        IEnumerable<long> GetGroupZoneIds(SaleZoneGroupSettings saleZoneGroup);
    }
}
