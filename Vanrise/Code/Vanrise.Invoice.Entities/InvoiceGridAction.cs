using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceGridAction
    {
        public string ActionTypeName { get; set; }
        public InvoiceGridActionSettings Settings { get; set; }
    }
}
