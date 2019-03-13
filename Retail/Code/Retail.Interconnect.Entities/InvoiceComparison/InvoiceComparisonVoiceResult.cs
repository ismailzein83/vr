using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Interconnect.Entities
{
    public class InvoiceComparisonVoiceResult
    {
        public string Destination { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string Currency { get; set; }
        public Decimal? SystemRate { get; set; }
        public Decimal? ProviderRate { get; set; }
        public Decimal? SystemDuration { get; set; }
        public Decimal? ProviderDuration { get; set; }
        public Decimal? SystemAmount { get; set; }
        public Decimal? ProviderAmount { get; set; }
        public Decimal? SystemCalls { get; set; }
        public Decimal? ProviderCalls { get; set; }
        public decimal? DiffCallsPercentage { get; set; }

        public decimal? DiffCalls { get; set; }
        public Vanrise.Entities.LabelColor? DiffCallsColor { get; set; }
        public decimal? DiffDuration { get; set; }
        public decimal? DiffDurationPercentage { get; set; }

        public Vanrise.Entities.LabelColor? DiffDurationColor { get; set; }

        public decimal? DiffAmount { get; set; }
        public decimal? DiffAmountPercentage { get; set; }

        public Vanrise.Entities.LabelColor? DiffAmountColor { get; set; }

        public ComparisonResult Result { get; set; }
        public Vanrise.Entities.LabelColor ResultColor { get; set; }
    }

    public class InvoiceComparisonVoiceResultDetail
    {
        public InvoiceComparisonVoiceResult Entity { get; set; }
        public string ResultDescription { get; set; }
        public string ResultTooltipDescription { get; set; }
    }

}
