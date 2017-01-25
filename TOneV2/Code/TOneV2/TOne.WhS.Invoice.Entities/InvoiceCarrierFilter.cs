using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vanrise.Invoice.Entities;
namespace TOne.WhS.Invoice.Entities
{
    public enum InvoiceCarrierType  { Profile=0,Account=1 }
    public class InvoiceCarrierFilter
    {
        public List<InvoiceCarrierType> CarrierTypes { get; set; }
        public List<IInvoicePartnerFilter> Filters { get; set; }
        public bool GetCustomers { get; set; }
        public bool GetSuppliers { get; set; }
    }
}
