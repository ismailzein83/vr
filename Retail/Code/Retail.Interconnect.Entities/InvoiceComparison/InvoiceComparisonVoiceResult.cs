using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Retail.Interconnect.Entities
{
    public enum ComparisonResult
    {
        [Description("Missing System")]
        MissingSystem = 1,

        [Description("Missing")]
        MissingProvider = 2,

        [Description("Identical")]
        Identical = 3,

        [Description("Minor Difference")]
        MinorDiff = 4,

        [Description("Major Difference")]
        MajorDiff = 5
    }
    public class InvoiceComparisonVoiceResult
    {
    }

    public class InvoiceComparisonVoiceResultDetail
    {
        public InvoiceComparisonVoiceResult Entity { get; set; }
        public string ResultDescription { get; set; }
        public string ResultTooltipDescription { get; set; }
    }

}
