using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class StateBackupCleanupContext : IStateBackupCleanupContext
    {
        public IEnumerable<long> SaleZoneIds { get; set; }

        public IEnumerable<long> SupplierZoneIds { get; set; }
    }
}
