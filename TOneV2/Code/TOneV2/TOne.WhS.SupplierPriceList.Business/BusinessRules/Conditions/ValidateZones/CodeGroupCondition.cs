using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;
using Vanrise.BusinessProcess.Entities;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class CodeGroupCondition : BusinessRuleCondition
    {
        public override bool ShouldValidate(IRuleTarget target)
        {
            return (target as ImportedCode != null);
        }

        public override bool Validate(IBusinessRuleConditionValidateContext context)
        {
            ImportedCode code = context.Target as ImportedCode;
            var result =  code.CodeGroup != null;

            if(result == false)
                context.Message = string.Format("Can not add Code {0} because it is not assigned to a code group", code.Code);

            return result;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Code {0} not assigned to a code group", (target as ImportedCode).Code);
        }
    }
}
