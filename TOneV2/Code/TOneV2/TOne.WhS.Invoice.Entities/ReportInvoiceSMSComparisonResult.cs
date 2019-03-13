using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class ReportInvoiceSMSComparisonResult
    {
        public Decimal SystemSMSs { get; set; }
        public Decimal ProviderSMSs { get; set; }
        public Decimal Difference { get; set; }
        public Decimal DiffPercentage { get; set; }
        public Decimal Average { get; set; }
    }
}
