using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceUIGridColumn
    {
        public string Header { get; set; }

        public InvoiceField Field { get; set; }

        public string CustomFieldName { get; set; }
    }
}
