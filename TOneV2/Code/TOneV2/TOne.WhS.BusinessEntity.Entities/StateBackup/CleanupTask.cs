using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public enum CleanupDataType { SaleZone = 1, SupplierZone = 2}

    public abstract class CleanupTask
    {
        public abstract void Cleanup(Dictionary<CleanupDataType, object> data);
    }
}
