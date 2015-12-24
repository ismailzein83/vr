using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public interface IBusinessRuleActionExecutionContext
    {
        IRuleTarget Target { get; set; }

        bool StopExecution { get; set; }
    }
}
