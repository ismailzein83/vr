using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class CodeRuleTarget : IRuleTarget
    {
        public ImportedCode ImportedCode { get; set; }

        public MessageSeverity Severity { get; set; }

        public string Message { get { return ""; } }

        public void SetExecluded()
        {
            throw new NotImplementedException();
        }


        public bool IsExecluded { get; set; }
    }
}
