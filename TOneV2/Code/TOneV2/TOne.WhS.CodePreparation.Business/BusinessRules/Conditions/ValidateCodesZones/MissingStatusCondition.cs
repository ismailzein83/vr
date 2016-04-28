using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Business
{
    public class MissingStatusCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedCode != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            if (context.Target == null)
                throw new ArgumentNullException("Target");

            ImportedCode importedData = context.Target as ImportedCode;

            return importedData.Status != null;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("code {0} has a missing status", (target as ImportedCode).Code);
        }
    }
}
