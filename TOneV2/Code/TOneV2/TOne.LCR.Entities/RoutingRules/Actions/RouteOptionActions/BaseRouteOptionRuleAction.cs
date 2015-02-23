using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public class BaseRouteOptionRuleAction
    {
        public virtual int ActionPriority
        {
            get
            {
                return 0;
            }
        }

        public virtual RouteRuleOptionExecutionResult Execute(RouteSupplierOption option, RouteDetail routeDetail, BaseRouteRule ruleDefinition)
        {
            return null;
        }
    }

    public class RouteRuleOptionExecutionResult
    {
        public bool IsInactive { get; set; }

        public bool RemoveOption { get; set; }
    }
}
