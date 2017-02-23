using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerPortal.Invoice.Entities
{
    public class InvoiceViewerTypeInfoFilter
    {
        public IEnumerable<IInvoiceViewerTypeInfoFilter> Filters { get; set; }
    }
}
