using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess
{
    public class BusinessRuleActionExecutionContext : IBusinessRuleActionExecutionContext
    {
        public bool StopExecution { get; set; }

        public IRuleTarget Target { get; set; }
    }
}
