using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vanrise.Invoice.Entities
{
    public class InvoiceGridSettings 
    {
        public List<InvoiceGridAction> InvoiceGridActions { get; set; }
        public List<InvoiceUIGridColumn> MainGridColumns { get; set; }
    }
}
