using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public abstract class BaseRouteOptionAlertCondition
    {
        public virtual bool IsViolated(RouteSupplierOption option, RouteDetail routeDetail)
        {
            return false;
        }
    }
}
