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
            if (target as CodeToAdd != null)
                return true;
            else if (target as CodeToClose != null)
                return true;
            else
                return false;

        }

        public override bool Validate(IRuleTarget target)
        {
            var returnedResult = false;
            if (target as CodeToAdd != null)
            {
                CodeToAdd code = target as CodeToAdd;

                if (code == null)
                    return false;

                returnedResult = code.CodeGroup != null;
            }
            else if (target as CodeToClose != null)
            {
                CodeToClose code = target as CodeToClose;

                if (code == null)
                    return false;

                returnedResult = code.CodeGroup != null;
            }
            return returnedResult;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Code {0} has no code group assigned", (target as CodeToAdd).Code);
        }
    }
}
