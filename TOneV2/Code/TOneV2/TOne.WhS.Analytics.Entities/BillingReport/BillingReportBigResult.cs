using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.WhS.Analytics.Entities
{
    public class BillingReportBigResult : Vanrise.Entities.BigResult<BillingReportRecord>
    {
        public BillingReportRecord Summary { get; set; }
        public string DimensionTitle { get; set; }
    }
}
