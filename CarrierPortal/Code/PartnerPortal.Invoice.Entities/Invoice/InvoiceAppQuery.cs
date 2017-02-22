using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;

namespace PartnerPortal.Invoice.Entities
{
    public class InvoiceAppQuery : InvoiceQuery
    {
        public Guid InvoiceViewerTypeId { get; set; }
    }
}
