using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.CodePreparation.Entities.CP.Processing;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.CodePreparation.Business
{
    public class CodeGroupCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as CodeToAdd != null);
        }

        public override bool Validate(IRuleTarget target)
        {
            CodeToAdd code = target as CodeToAdd;

            if (code == null)
                return false;

            return code.CodeGroup != null;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Code {0} has no code group assigned", (target as CodeToAdd).Code);
        }
    }
}
