using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public enum RuleActionType { Inform = 0, Warn = 1, ExecludeItem = 2, StopExecution = 3};

    public class BusinessRule
    {
        public BusinessRuleCondition Condition { get; set; }

        public BusinessRuleAction Action { get; set; }
        
    }
}
