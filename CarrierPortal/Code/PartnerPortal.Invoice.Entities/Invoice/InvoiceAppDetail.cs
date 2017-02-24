using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PartnerPortal.Invoice.Entities
{
    public class InvoiceAppDetail
    {
        public Vanrise.Invoice.Entities.Invoice Entity { get; set; }
        public bool HasNote { get; set; }
        public List<InvoiceDetailObject> Items { get; set; }
        public bool Lock { get; set; }
        public bool Paid { get; set; }
        public string PartnerName { get; set; }
        public string UserName { get; set; }
        public List<Guid> ActionsIds { get; set; }
    }
    public class InvoiceDetailObject
    {
        public string Description { get; set; }
        public string FieldName { get; set; }
        public object Value { get; set; }
    }
}
