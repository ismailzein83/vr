using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Security.Entities;

namespace PartnerPortal.Invoice.Entities
{
    public class InvoiceViewSettings : ViewSettings
    {
        public List<InvoiceViewItem> InvoiceViewItems { get; set; }

        public override string GetURL(View view)
        {
            return String.Format("#/viewwithparams/PartnerPortal_Invoice/Elements/Invoice/Views/InvoiceManagement/{{\"viewId\":\"{0}\"}}", view.ViewId);
        }
    }
    public class InvoiceViewItem
    {
        public Guid InvoiceViewerTypeId { get; set; }
    }
}
