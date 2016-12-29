using System;
using System.Activities;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;

namespace Vanrise.BusinessProcess
{
    public class BusinessRuleConditionValidateContext : IBusinessRuleConditionValidateContext
    {
        ActivityContext _activityContext;

        public BusinessRuleConditionValidateContext(ActivityContext context)
        {
            this._activityContext = context;
        }

        public IRuleTarget Target { get; set; }

        public string Message { get; set; }

        public T GetExtension<T>() where T : class
        {
            return _activityContext.GetExtension<T>();
        }
    }
}
