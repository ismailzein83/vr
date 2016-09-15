using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class SupplierInvoiceDetails
    {
        public int TotalNumberOfCalls { get; set; }
        public Double SupplierAmount { get; set; }
        public decimal Duration { get; set; }
        public string PartnerType { get; set; }
        public SupplierInvoiceDetails() { }
        public IEnumerable<SupplierInvoiceDetails> GetSupplierInvoiceDetailsRDLCSchema()
        {
            return null;
        }
    }
}
