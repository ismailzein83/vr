using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Interconnect.Entities
{
    public class ReportComparisonInvoiceDetail
    {
        public DateTime IssueDate { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public bool IsVoiceCompare { get; set; }
        public bool IsSMSCompare { get; set; }

    }
}
