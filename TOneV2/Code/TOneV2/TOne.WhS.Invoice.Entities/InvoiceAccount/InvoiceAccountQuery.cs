using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class InvoiceAccountQuery
    {
        public int? CarrierAccountId { get; set; }
        public int? CarrierProfileId { get; set; }
    }
}
