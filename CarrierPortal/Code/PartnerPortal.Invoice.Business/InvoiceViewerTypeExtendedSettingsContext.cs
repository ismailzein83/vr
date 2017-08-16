using PartnerPortal.Invoice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerPortal.Invoice.Business
{
    class InvoiceViewerTypeExtendedSettingsContext : IInvoiceViewerTypeExtendedSettingsContext
    {
        public InvoiceViewerTypeSettings InvoiceViewerTypeSettings { get; set; }
        public int UserId { get; set; }
    }
}
