using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.LCR.Entities;

namespace TOne.LCR.Business
{
    public class RouteRuleManager
    {
        public RuleActionExecutionPath GetExecutionPath()
        {
            return _executionPath;
        }

        static RuleActionExecutionPath _executionPath = BuildExecutionPath();

        private static RuleActionExecutionPath BuildExecutionPath()
        {
            RuleActionExecutionStep blockStep = new RuleActionExecutionStep { Action = new BlockRouteAction(), IsEndAction = true };
            RuleActionExecutionStep overrideStep = new RuleActionExecutionStep { Action = new OverrideRouteAction(), IsEndAction = true };
            RuleActionExecutionStep buildLCRStep = new RuleActionExecutionStep { Action = new BuildLCRRouteAction() };
            RuleActionExecutionStep priorityStep = new RuleActionExecutionStep { Action = new PriorityRouteAction() };
            RuleActionExecutionStep getTopOptionsStep = new RuleActionExecutionStep { Action = new GetTopOptionsRouteAction() };
            RuleActionExecutionStep checkNoOptionsStep = new RuleActionExecutionStep { Action = new CheckNoOptionsRouteAction() };
            RuleActionExecutionStep applyPercentageStep = new RuleActionExecutionStep { Action = new ApplyPercentageRouteAction() };

            RuleActionExecutionPath executionPath = new RuleActionExecutionPath { FirstStep = blockStep };
            blockStep.NextStep = overrideStep;
            overrideStep.NextStep = buildLCRStep;
            buildLCRStep.NextStep = priorityStep;
            priorityStep.NextStep = getTopOptionsStep;  
            getTopOptionsStep.NextStep = checkNoOptionsStep;
            checkNoOptionsStep.NextStep = applyPercentageStep;
            return executionPath;
        }
    }
}
