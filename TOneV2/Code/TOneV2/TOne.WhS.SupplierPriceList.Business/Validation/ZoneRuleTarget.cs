using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TOne.WhS.SupplierPriceList.Entities;
using TOne.WhS.SupplierPriceList.Entities.SPL;

namespace TOne.WhS.SupplierPriceList.Business
{
    public class ZoneRuleTarget : IRuleTarget
    {
        public ZoneRuleTarget(ImportedZone importedZone)
        {
            this.ImportedZone = importedZone;
            this.IsExecluded = false;
        }

        public ImportedZone ImportedZone { get; set; }

        public MessageSeverity Severity { get; set; }

        public string Message { get { return ""; } }

        public void SetExecluded()
        {
            this.ImportedZone.SetExecluded();

            if(this.ImportedZone.ImportedCodes != null)
            {
                foreach (ImportedCode code in this.ImportedZone.ImportedCodes)
                {
                    code.SetExecluded();
                }
            }
        }

        public bool IsExecluded { get; set; }
    }
}
