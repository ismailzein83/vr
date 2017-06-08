using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceDataSourceItem
    {
        public InvoiceDataSourceItem() { }
        public string CurrencyName { get; set; }
        public decimal  Amount { get; set; }
        public string SerialNumber { get; set; }
        public DateTime CreatedTime { get; set; }

        public IEnumerable<InvoiceDataSourceItem> GetInvoiceDataSourceItemRDLCSchema()
        {
            return null;
        }
    }
}
