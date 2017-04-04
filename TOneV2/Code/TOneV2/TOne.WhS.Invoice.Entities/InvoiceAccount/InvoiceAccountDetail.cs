using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class InvoiceAccountDetail
    {
        public  InvoiceAccount Entity { get; set; }
        public string InvoiceTypeDescription { get; set; }
        public bool IsActive { get; set; }
    }
}
