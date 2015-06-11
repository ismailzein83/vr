using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TOne.Analytics.Entities
{
    public class CarrierSummaryDaily
    {
        public string Day { get; set; }
        public string CarrierID { get; set; }
        public int?Attempts { get; set; }
        public decimal? DurationNet { get; set; }
        public decimal? Duration { get; set; }
        public double? Net { get; set; }    


    }
}
