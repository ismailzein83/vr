using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class CodeGroupRule : BusinessRule
    {
        public override bool Validate(IRuleTarget target)
        {
            ImportedCode code = target as ImportedCode;
            return code.CodeGroup != null;
        }
    }
}
