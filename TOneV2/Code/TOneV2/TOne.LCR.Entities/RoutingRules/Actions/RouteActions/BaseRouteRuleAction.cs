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
        
        public virtual int ActionPriority
        {
            get
            {
                return 0;
            }
        }

        public virtual RouteRuleExecutionResult Execute(RouteDetail route, BaseRouteRule ruleDefinition, SupplierZoneRates supplierZoneRates)
        {
            return null;
        }
    }

    public class RouteRuleExecutionResult
    {
        public bool IsInactive { get; set; }
    }
}
