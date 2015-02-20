using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.LCR.Entities
{
    public enum RouteRuleExecutionStage {  BeforeLCR, AfterLCR, AtTheEnd}

    public abstract class BaseRouteRuleAction
    {
        public abstract string ActionDisplayName { get; }

        public virtual RouteRuleExecutionStage ExecutionStage
        {
            get
            {
                return RouteRuleExecutionStage.BeforeLCR;
            }
        }



        public virtual int GetActionPriority()
        {
            return 0;
        }

        public virtual RouteRuleExecutionResult Execute(RouteDetail initialRoute, BaseRouteRule ruleDefinition)
        {
            return null;
        }
    }

    public class RouteRuleExecutionResult
    {
        public RouteDetail FinalRoute { get; set; }

        public bool IsInactive { get; set; }
    }
}
