using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.BusinessEntity.Business
{
    public class BusinessEntityCleanupTask : StateBackupCleanupTask
    {
        public override void Cleanup(IStateBackupCleanupContext context)
        {
            StateBackupBusinessEntitiesCleanupManager manager = new StateBackupBusinessEntitiesCleanupManager();

            if (context.SaleZoneIds != null || context.SupplierZoneIds != null)
                manager.CleanupGenericRules(context);

            if (context.SaleZoneIds != null)
                manager.CleanupRoutingProducts(context);
        }
    }
}
