using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceViewSettings : Vanrise.Security.Entities.ViewSettings
    {
        public Guid InvoiceTypeId { get; set; }
        public int TypeId { get; set; }
        public override string GetURL(Security.Entities.View view)
        {
            return String.Format("#/viewwithparams/VR_Invoice/Views/GenericInvoiceManagement/{{\"invoiceTypeId\":\"{0}\"}}", this.InvoiceTypeId);
        }
    }
}
