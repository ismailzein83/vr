using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using TOne.WhS.CodePreparation.Entities.Processing;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Business
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
                context.Message = string.Format("Zone {0} has a missing code", importedData.ZoneName);

            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Zone {0} has a missing code", (target as ImportedCode).ZoneName);
        }
    }
}
