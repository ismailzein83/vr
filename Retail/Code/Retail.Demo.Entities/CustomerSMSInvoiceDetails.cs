using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Demo.Entities
{
    public class CustomerSMSInvoiceDetails
    {
        public Decimal Amount { get; set; }
        public Decimal NumberOfSMS { get; set; }
        public int CurrencyId { get; set; }
        public string CurrencyIdDescription { get; set; }
        public CustomerSMSInvoiceDetails()
        {

        }
        public IEnumerable<CustomerSMSInvoiceDetails> GetCustomerSMSInvoiceDetailsRDLCSchema()
        {
            return null;
        }

    }
}
