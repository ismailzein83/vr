using PartnerPortal.Invoice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerPortal.Invoice.Business
{
    public class InvoiceViewerTypeInfoFilterContext : IInvoiceViewerTypeInfoFilterContext
    {
        public Guid InvoiceViewerTypeId { get; set; }
    }
}
