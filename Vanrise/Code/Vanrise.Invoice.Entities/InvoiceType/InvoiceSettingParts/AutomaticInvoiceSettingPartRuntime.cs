using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class AutomaticInvoiceSettingPartRuntime
    {
        public List<AutomaticInvoiceAction> AutomaticInvoiceActions { get; set; }
        public List<InvoiceAttachment> InvoiceAttachments { get; set; }
    }
}
