using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class InvoiceComparisonResultDetail
    {
        public InvoiceComparisonResult Entity { get; set; }
        public string ResultDescription { get; set; }
        public string ResultTooltipDescription { get; set; }
    }
}
