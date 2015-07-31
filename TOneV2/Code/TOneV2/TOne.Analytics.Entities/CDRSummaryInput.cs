using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class CDRSummaryInput
    {
       // public BillingCDRMeasures OrderBy { get; set; }
        public CDRFilter Filter { get; set; }
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public int Size { get; set; }
        public BillingCDROptionMeasures CDROption { get; set; }
    }
}
