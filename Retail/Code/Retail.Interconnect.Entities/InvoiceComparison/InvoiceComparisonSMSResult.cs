using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Interconnect.Entities
{
    public class InvoiceComparisonSMSResult
    {
    }


    public class InvoiceComparisonSMSResultDetail
    {
        public InvoiceComparisonSMSResult Entity { get; set; }
        public string ResultDescription { get; set; }
        public string ResultTooltipDescription { get; set; }
    }

}
