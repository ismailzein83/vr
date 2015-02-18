using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public abstract class BaseRouteRuleAction
    {
        public abstract string ActionDisplayName { get; }

        public virtual int GetActionPriority()
        {
            return 0;
        }

        public virtual bool NoNeedForLCR
        {
            get
            {
                return false; 
            }
        }

        public virtual bool GetFinalRoute(RouteDetail initialRoute, BaseRouteRule ruleDefinition, out RouteDetail finalRoute)
        {
            finalRoute = null;
            return true;
        }
    }
}
