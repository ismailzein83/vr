using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public abstract class SuppliersWithZonesGroupSettings
    {
        public abstract Guid ConfigId { get; }
        public abstract IEnumerable<SupplierWithZones> GetSuppliersWithZones(ISuppliersWithZonesGroupContext context);
        public abstract string GetDescription(ISuppliersWithZonesGroupContext context);
    }    
}
 