using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public interface IBusinessRuleConditionValidateContext
    {
        IRuleTarget Target { get; set; }
        string Message { get; set; }
        T GetExtension<T>() where T : class;
    }
}
