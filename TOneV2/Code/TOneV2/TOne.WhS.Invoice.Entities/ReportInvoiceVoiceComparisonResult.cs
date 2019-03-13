using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Invoice.Entities
{
    public class ReportInvoiceVoiceComparisonResult
    {
        public Decimal SystemDuration { get; set; }
        public Decimal ProviderDuration { get; set; }
        public Decimal Difference { get; set; }
        public Decimal DiffPercentage { get; set; }
        public Decimal Average { get; set; }

    }
}
