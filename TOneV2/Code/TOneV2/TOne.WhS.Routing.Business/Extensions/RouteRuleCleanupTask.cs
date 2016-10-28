using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Business.Extensions
{
    public class RouteRuleCleanupTask : StateBackupCleanupTask
    {
        public override void Cleanup(IStateBackupCleanupContext context)
        {
            StateBackupRoutingCleanupManager manager = new StateBackupRoutingCleanupManager();
            
            if(context.SaleZoneIds != null)
                manager.CleanupRouteRules(context);

            if (context.SaleZoneIds != null || context.SupplierZoneIds != null)
                manager.CleanupRouteOptionRules(context);
        }
    }
}
