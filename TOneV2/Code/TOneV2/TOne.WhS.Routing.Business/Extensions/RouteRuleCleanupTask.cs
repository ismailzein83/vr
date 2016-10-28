using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.BusinessEntity.Entities;

namespace TOne.WhS.Routing.Business.Extensions
{
    public class RouteRuleCleanupTask : CleanupTask
    {
        public override void Cleanup(Dictionary<CleanupDataType, object> data)
        {
            if(data.ContainsKey(CleanupDataType.SaleZone))
                new RouteRuleCleanupManager().Cleanup(data);

            if (data.ContainsKey(CleanupDataType.SaleZone) || data.ContainsKey(CleanupDataType.SupplierZone))
                new RouteOptionRuleCleanupManager().Cleanup(data);
        }
    }
}
