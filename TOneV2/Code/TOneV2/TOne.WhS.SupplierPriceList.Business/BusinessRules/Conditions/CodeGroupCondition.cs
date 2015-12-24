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

        public override bool Validate(IRuleTarget target)
        {
            ImportedCode code = target as ImportedCode;

            if (code == null)
                return false;

            return code.CodeGroup != null;
        }

        public override string GetMessage(IRuleTarget target)
        {
            return string.Format("Code {0} has no code group assigned", (target as ImportedCode).Code);
        }
    }
}
