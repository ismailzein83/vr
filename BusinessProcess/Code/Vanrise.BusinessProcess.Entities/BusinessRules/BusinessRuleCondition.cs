using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.BusinessProcess.Entities
{
    public abstract class BusinessRuleCondition
    {
        public abstract bool ShouldValidate(IRuleTarget target);

        public abstract bool Validate(IBusinessRuleConditionValidateContext context);

        public abstract string GetMessage(IRuleTarget target);
    }
}
