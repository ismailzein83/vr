using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public interface ISuppliersWithZonesGroupContext : IContext
    {
        SupplierFilterSettings FilterSettings { get; set; }

        IEnumerable<SupplierWithZones> GetSuppliersWithZones(SuppliersWithZonesGroupSettings group);
    }
}
