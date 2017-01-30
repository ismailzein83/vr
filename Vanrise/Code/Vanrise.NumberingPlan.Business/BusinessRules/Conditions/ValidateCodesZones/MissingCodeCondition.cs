using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.BusinessProcess.Entities;
using Vanrise.NumberingPlan.Entities;

namespace Vanrise.NumberingPlan.Business
{
    public class MissingCodeCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedCode != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {

            ImportedCode importedData = context.Target as ImportedCode;
            var result = !string.IsNullOrEmpty(importedData.Code);

            if (result == false)
                context.Message = string.Format("Can not add Zone {0} because it has a missing code", importedData.ZoneName);

            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has a missing code", (target as ImportedCode).ZoneName);
        }
    }
}
