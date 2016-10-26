using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class SupplierZoneGroupCleanupContext : ISupplierZoneGroupCleanupContext
    {
        public SupplierZoneGroupCleanupResult Result { get; set; }

        public IEnumerable<long> DeletedSupplierZoneIds { get; set; }
    }
}
