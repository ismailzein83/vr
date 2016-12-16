using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Invoice.Entities
{
    public class RDLCSubscriberInvoiceItemDetails
    {
        public decimal Amount { get; set; }

        public Guid ServiceTypeId { get; set; }

        public RDLCSubscriberInvoiceItemDetails()
        {
                
        }

        public IEnumerable<RDLCSubscriberInvoiceItemDetails> GetRDLCSubscriberInvoiceItemDetailsSchema()
        {
            return null;
        }
    }
}
