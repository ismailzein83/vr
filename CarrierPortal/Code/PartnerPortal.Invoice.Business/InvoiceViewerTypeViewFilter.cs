using PartnerPortal.Invoice.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Business;

namespace PartnerPortal.Invoice.Business
{
    public class InvoiceViewerTypeViewFilter : IInvoiceViewerTypeInfoFilter
    {
        public Guid ViewId { get; set; }

        public bool IsExcluded(IInvoiceViewerTypeInfoFilterContext context)
        {
            ViewManager viewManager = new ViewManager();
            var invoiceViewerTypeViewSettings = viewManager.GetView(this.ViewId).Settings as InvoiceViewSettings;
            if (invoiceViewerTypeViewSettings.InvoiceViewItems.Any(x=>x.InvoiceViewerTypeId == context.InvoiceViewerTypeId))
                return false;
            return true;
        }
    }
}
