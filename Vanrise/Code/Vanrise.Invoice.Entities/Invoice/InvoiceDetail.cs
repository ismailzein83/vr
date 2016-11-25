using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceDetail
    {
        public Invoice Entity { get; set; }
        public string PartnerName { get; set; }
        public List<string> SectionsTitle { get; set; }
        public List<InvoiceGridAction> ActionTypeNames { get; set; }
        public Boolean Paid { get; set; }
        public Boolean Lock { get; set; }
        public string UserName { get; set; }
        public Boolean HasNote { get; set; }
        public List<InvoiceDetailObject> Items { get; set; }

    }
    public class InvoiceDetailObject
    {
        public string FieldName { get; set; }
        public Object Value { get; set; }
        public string Description { get; set; }
    }
}
