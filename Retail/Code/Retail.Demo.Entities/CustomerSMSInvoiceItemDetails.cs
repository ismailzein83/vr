using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Demo.Entities
{
    public class CustomerSMSInvoiceItemDetails
    {
        public long CustomerId { get; set; }
        public string CustomerIdDescription { get; set; }
        public Decimal Amount { get; set; }
        public int NumberOfSMS { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyIdDescription { get; set; }
        public long DestinationZoneId { get; set; }
        public String DestinationZoneIdDescription { get; set; }
        public CustomerSMSInvoiceItemDetails()
        {

        }
        public IEnumerable<CustomerSMSInvoiceItemDetails> GetCustomerSMSInvoiceItemDetailsRDLCSchema()
        {
            return null;
        }
    }
}
