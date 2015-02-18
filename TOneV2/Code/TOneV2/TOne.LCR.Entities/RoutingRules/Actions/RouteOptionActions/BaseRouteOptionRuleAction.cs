using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class BaseRouteOptionRuleAction
    {
        public virtual bool GetFinalOption(SupplierRouteDetail initialOption, RouteDetail routeDetail, BaseRouteRule ruleDefinition, out SupplierRouteDetail finalOption)
        {
            finalOption = null;
            return true;
        }
    }
}
