using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class CodeRuleTarget : BusinessRule<ImportedCode>
    {
        public override bool isValid()
        {
            foreach (ImportedCode code in base.data)
            {
                if (code.CodeGroupId == null)
                    return false;
            }

            return true;
        }

        public override void SetExecluded()
        {
            throw new NotImplementedException();
        }
    }
}
