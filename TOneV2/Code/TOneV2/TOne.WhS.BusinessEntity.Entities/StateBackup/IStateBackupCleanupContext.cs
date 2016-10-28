using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.BusinessEntity.Entities
{
    public interface IStateBackupCleanupContext
    {
        IEnumerable<long> SaleZoneIds { get; set; }

        IEnumerable<long> SupplierZoneIds { get; set; }
    }
}
