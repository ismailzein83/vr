using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace Retail.Interconnect.Business
{
    public class SMSInvoiceUIGridColumnFilter : InvoiceUIGridColumnFilter
    {
        public override Guid ConfigId
        {
            get { return new Guid("15ceed9b-0c8f-4446-8bbe-92e84857ae31"); }
        }

        public override bool IsFilterMatch(IInvoiceUIGridColumnFilterContext context)
        {
            InterconnectModuleManager interconnectModuleManager = new InterconnectModuleManager();
            return interconnectModuleManager.IsSMSModuleEnabled();
        }
    }
}
